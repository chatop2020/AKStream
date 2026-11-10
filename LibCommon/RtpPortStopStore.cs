using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibCommon.Structs;
using Newtonsoft.Json;

namespace LibCommon
{
    /// <summary>
    /// 保存接收端口的停流隔离及冷却状态。独立 JSON 与启动时清空的临时在线状态分开维护；
    /// 读写失败向上抛出，分配方不得因此放行端口。
    /// </summary>
    public sealed class RtpPortStopStore
    {
        private readonly string _path;
        private readonly object _sync = new object();

        /// <summary>绑定状态文件，不读取或修改项目配置。</summary>
        /// <param name="path">持久化 JSON 文件的完整路径，不能与其他 Keeper 共用。</param>
        public RtpPortStopStore(string path) { _path = path; }

        /// <summary>检查端口是否没有停流隔离，或已经完成两侧清理且冷却结束。</summary>
        /// <param name="port">待分配的接收端口。</param>
        /// <param name="nowUtc">本次判断使用的 UTC 时间。</param>
        /// <returns>只有明确允许复用时返回 true；损坏的状态文件会抛出异常。</returns>
        public bool CanReuse(ushort port, DateTime nowUtc)
        {
            lock (_sync)
            {
                var item = Read().Find(x => x.Port == port);
                return item == null || item.ReuseAfter.HasValue && item.ReuseAfter.Value <= nowUtc;
            }
        }

        /// <summary>在发送 BYE 或关闭接收服务前持久化隔离，同一停流任务可重复调用。</summary>
        /// <param name="stop">包含原始 dialog 的停流记录；不接受零端口或空会话标识。</param>
        /// <returns>记录已落盘时返回 true；端口仍属于另一清理任务时返回 false。</returns>
        public bool Hold(RtpPortStop stop)
        {
            if (stop.Port == 0 || string.IsNullOrWhiteSpace(stop.StopId))
                throw new ArgumentException("停流隔离必须提供接收端口和 SIP Call-ID。");
            lock (_sync)
            {
                var items = Read();
                var old = items.Find(x => x.Port == stop.Port);
                if (old != null && old.StopId == stop.StopId) return !old.ReuseAfter.HasValue;
                if (old != null && (!old.ReuseAfter.HasValue || old.ReuseAfter > DateTime.UtcNow)) return false;
                if (old != null) items.Remove(old);
                var copy = JsonConvert.DeserializeObject<RtpPortStop>(JsonConvert.SerializeObject(stop))!;
                copy.ByeConfirmed = false;
                copy.ReceiverClosed = false;
                copy.ReuseAfter = null;
                copy.Attempts = 0;
                copy.NextRetryAt = DateTime.UtcNow.AddSeconds(30);
                items.Add(copy);
                Write(items);
                return true;
            }
        }

        /// <summary>合并同一任务的清理结果；两侧完成后才持久化冷却截止时间。</summary>
        /// <param name="stop">本次 BYE、端口关闭结果及失败原因。</param>
        /// <param name="cooldownSeconds">现有 RtpPortCdTime，单位为秒，负值按零处理。</param>
        /// <returns>任务匹配且更新成功时返回 true；过期任务不能改变新任务。</returns>
        public bool Complete(RtpPortStop stop, int cooldownSeconds)
        {
            lock (_sync)
            {
                var items = Read();
                var item = items.Find(x => x.Port == stop.Port && x.StopId == stop.StopId);
                if (item == null) return false;
                if (item.ReuseAfter.HasValue) return true; // 重复结果不能延长或缩短已开始的冷却。
                item.ByeConfirmed |= stop.ByeConfirmed;
                // 关闭状态必须采用最新观测；不能用上次成功掩盖本次仍有监听或查询失败。
                item.ReceiverClosed = stop.ReceiverClosed;
                item.Attempts++;
                item.LastError = stop.LastError;
                item.NextRetryAt = DateTime.UtcNow.AddSeconds(Math.Min(300, item.Attempts * 30));
                if (item.ByeConfirmed && item.ReceiverClosed)
                {
                    item.ReuseAfter = DateTime.UtcNow.AddSeconds(Math.Max(0, cooldownSeconds));
                    item.InviteRequest = "";
                    item.InviteResponse = "";
                }
                Write(items);
                return true;
            }
        }

        /// <summary>取得尚未完成两侧清理的副本，自动清理不能删除这些记录。</summary>
        /// <returns>包含内部 dialog 的待重试记录；调用方必须限制访问且不得打印信令原文。</returns>
        public List<RtpPortStop> Pending()
        {
            lock (_sync) return Read().Where(x => !x.ReuseAfter.HasValue).ToList();
        }

        /// <summary>一次读取本轮不可复用端口，避免分配循环反复读取包含 dialog 的文件。</summary>
        /// <param name="nowUtc">本次分配的 UTC 时间。</param>
        /// <returns>仍隔离或仍在冷却中的接收端口集合。</returns>
        public HashSet<ushort> UnavailablePorts(DateTime nowUtc)
        {
            lock (_sync) return Read().Where(x => !x.ReuseAfter.HasValue || x.ReuseAfter.Value > nowUtc)
                .Select(x => x.Port).ToHashSet();
        }

        /// <summary>删除已经完成清理且冷却到期的记录，未确认的隔离不按年龄自动删除。</summary>
        /// <param name="nowUtc">本轮维护的 UTC 时间。</param>
        /// <returns>已删除的记录数；持久化失败时上抛，原文件保持不变。</returns>
        public int PruneExpired(DateTime nowUtc)
        {
            lock (_sync)
            {
                var items = Read();
                var removed = items.RemoveAll(x => x.ByeConfirmed && x.ReceiverClosed &&
                    x.ReuseAfter.HasValue && x.ReuseAfter.Value <= nowUtc);
                if (removed > 0) Write(items);
                return removed;
            }
        }

        /// <summary>读取唯一权威文件，内容不完整时拒绝按空状态继续运行。</summary>
        private List<RtpPortStop> Read()
        {
            if (!File.Exists(_path)) return new List<RtpPortStop>();
            var items = JsonConvert.DeserializeObject<List<RtpPortStop>>(File.ReadAllText(_path));
            if (items == null || items.Any(x => x.Port == 0 || string.IsNullOrWhiteSpace(x.StopId)) ||
                items.Any(x => x.ReuseAfter.HasValue && (!x.ByeConfirmed || !x.ReceiverClosed)) ||
                items.Select(x => x.Port).Distinct().Count() != items.Count)
                throw new InvalidDataException("RTP 停流隔离状态损坏，禁止继续分配接收端口。");
            return items;
        }

        /// <summary>先落盘临时文件再同目录替换，写入失败时保留原隔离记录并上抛异常。</summary>
        private void Write(List<RtpPortStop> items)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_path))!);
            var temporary = _path + ".tmp";
            try
            {
                using (var file = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items, Formatting.Indented));
                    file.Write(data, 0, data.Length);
                    file.Flush(true);
                }
                File.Move(temporary, _path, true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
