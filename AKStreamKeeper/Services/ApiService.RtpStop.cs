using System;
using System.Collections.Generic;
using LibCommon.Structs;

namespace AKStreamKeeper.Services
{
    public static partial class ApiService
    {
        /// <summary>发送 BYE 前持久化端口隔离，只有同一 SIP 会话可以完成它。</summary>
        /// <param name="stop">包含接收端口、会话标识和重试所需 dialog 的记录。</param>
        /// <returns>隔离落盘成功时返回 true；端口归属冲突或写入失败时不放行。</returns>
        public static bool HoldRtpPortStop(RtpPortStop stop)
        {
            lock (Common._getRtpPortLock) return Common.RtpPortStops.Hold(stop);
        }

        /// <summary>记录本次清理结果，两侧均确认后开始既有 CdTime 冷却。</summary>
        /// <param name="stop">对应 StopId 的 BYE 和接收服务关闭结果。</param>
        /// <returns>结果被当前隔离任务接受时返回 true。</returns>
        public static bool CompleteRtpPortStop(RtpPortStop stop)
        {
            lock (Common._getRtpPortLock)
            {
                if (!Common.RtpPortStops.Complete(stop, Common.AkStreamKeeperConfig.RtpPortCdTime)) return false;
                if (stop.ByeConfirmed && stop.ReceiverClosed)
                {
                    var item = Common.PortInfoList.FindLast(x => x.Port == stop.Port);
                    if (item != null)
                    {
                        item.Useed = false;
                        item.DateTime = DateTime.Now;
                    }
                }
                return true;
            }
        }

        /// <summary>读取待重试的停流记录，普通端口自动清理不能移除它们。</summary>
        /// <returns>内部清理记录，包含 SIP dialog，不应写入日志。</returns>
        public static List<RtpPortStop> GetPendingRtpPortStops()
        {
            lock (Common._getRtpPortLock) return Common.RtpPortStops.Pending();
        }

        /// <summary>根据本轮有效接收端口列表更新占用，所有释放统一经过隔离检查并重置冷却起点。</summary>
        /// <param name="ports">成功取得的实际接收端口列表；空列表也是有效结果。</param>
        public static void CleanUnusedRtpPorts(List<ushort> ports)
        {
            lock (Common._getRtpPortLock)
            {
                foreach (var item in Common.PortInfoList)
                {
                    if (item == null || !item.Useed) continue;
                    if (ports.Contains(item.Port)) { item.DateTime = DateTime.Now; continue; }
                    if (DateTime.Now > item.DateTime.AddSeconds(Common.AkStreamKeeperConfig.RtpPortCdTime))
                        ReleaseRtpPort(item.Port);
                }
            }
        }
    }
}
