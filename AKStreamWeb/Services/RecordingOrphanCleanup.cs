using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebResponse.AKStreamKeeper;

namespace AKStreamWeb.Services
{
    /// <summary>文件检查的观测结果。缺失和不可访问都不是删除录像索引的授权。</summary>
    public enum RecordingFileObservation { Present, Missing, Unavailable }

    /// <summary>磁盘扫描结果。只有索引明确记录硬删除意图时，扫描才可以重试删除物理文件。</summary>
    public enum RecordingDiskOutcome { Indexed, UnindexedPreserved, DeletionDeferred, Deleted, DeleteFailed }

    /// <summary>
    /// 双向清理的安全边界：找不到文件只报告并保留索引，找不到索引只报告并保留文件。
    /// 只有同一媒体节点、同一路径已被明确标记 Deleted=true、Undo=false 时才允许重试删除。
    /// </summary>
    public sealed class RecordingOrphanCleanup
    {
        private readonly IFreeSql _db;

        /// <summary>复用现有录像索引数据库，不创建新表、不修改录像状态。</summary>
        /// <param name="db">当前环境的 FreeSql 数据库实例。</param>
        public RecordingOrphanCleanup(IFreeSql db) { _db = db; }

        /// <summary>解释 Keeper 检查结果，绝不依据 false 或访问错误删除索引。</summary>
        /// <param name="exists">文件存在检查的布尔返回值。</param>
        /// <param name="result">访问结果；错误或空结果表示不可核实。</param>
        /// <returns>存在、当前未找到或不可访问；后两者都需要保留原录像索引。</returns>
        public static RecordingFileObservation ObserveFile(bool exists, ResponseStruct result) =>
            result == null || result.Code != ErrorNumber.None ? RecordingFileObservation.Unavailable :
            exists ? RecordingFileObservation.Present : RecordingFileObservation.Missing;

        /// <summary>按媒体节点和路径检查索引，只重试已经明确硬删除的文件。</summary>
        /// <param name="mediaServerId">拥有文件的媒体节点，不能跨节点复用扫描结果。</param>
        /// <param name="path">文件完整路径。</param>
        /// <param name="storageHealthy">仅在该存储根目录健康已被确认时允许重试物理删除。</param>
        /// <param name="deleteFile">当前节点的删除操作，成功返回 true；调用前重新读取删除意图。</param>
        /// <returns>是否已索引、无索引保留、删除延期或明确删除的执行结果。</returns>
        public RecordingDiskOutcome InspectDiskFile(string mediaServerId, string path, bool storageHealthy,
            Func<string, bool> deleteFile)
        {
            var rows = Find(mediaServerId, path);
            if (rows.Count == 0) return RecordingDiskOutcome.UnindexedPreserved;
            // 时钟回拨或补录可能让同一路径出现多条索引，旧删除标记不能覆盖有效索引。
            if (rows.Any(x => x.Deleted != true || x.Undo != false)) return RecordingDiskOutcome.Indexed;
            if (!storageHealthy) return RecordingDiskOutcome.DeletionDeferred;
            // 扫描与执行之间可能发生恢复操作，不使用前一轮缓存的删除列表。
            rows = Find(mediaServerId, path);
            if (rows.Count == 0 || rows.Any(x => x.Deleted != true || x.Undo != false))
                return RecordingDiskOutcome.DeletionDeferred;
            return deleteFile(path) ? RecordingDiskOutcome.Deleted : RecordingDiskOutcome.DeleteFailed;
        }

        /// <summary>只在文件所属存储根目录下查询健康状态；空状态是未知，其他磁盘故障不能串用。</summary>
        /// <param name="path">待检查文件或目录的完整路径。</param>
        /// <param name="states">Keeper 上报的存储根目录状态，0 表示健康。</param>
        /// <returns>最长匹配根目录的状态；无对应信息或路径非法时返回 null。</returns>
        public static int? StorageStatus(string path, IDictionary<string, int> states)
        {
            if (states == null || states.Count == 0 || string.IsNullOrWhiteSpace(path)) return null;
            try
            {
                var full = Path.GetFullPath(path);
                var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                return states.Select(x => new { Root = Path.GetFullPath(x.Key).TrimEnd(Path.DirectorySeparatorChar), x.Value })
                    .Where(x => full.Equals(x.Root, comparison) || full.StartsWith(x.Root + Path.DirectorySeparatorChar, comparison))
                    .OrderByDescending(x => x.Root.Length).Select(x => (int?)x.Value).FirstOrDefault();
            }
            catch { return null; }
        }

        /// <summary>只把远端确认完成的软删除项转为不可恢复，错误或失败路径保留原删除队列。</summary>
        /// <param name="records">同一媒体节点本次尝试清理的软删除索引。</param>
        /// <param name="result">Keeper 返回的失败路径列表；空响应表示没有确认。</param>
        /// <param name="status">Keeper 调用状态，任何错误都不推进索引。</param>
        /// <returns>实际更新的索引条数；未被确认的项仍可重试或恢复。</returns>
        public int FinalizeSoftDeletes(IEnumerable<RecordFile> records, ResKeeperDeleteFileList result, ResponseStruct status)
        {
            if (result?.PathList == null || status == null || status.Code != ErrorNumber.None) return 0;
            var failures = new HashSet<string>(result.PathList, StringComparer.Ordinal);
            var ids = records.Where(x => !string.IsNullOrWhiteSpace(x.VideoPath) && !failures.Contains(x.VideoPath))
                .Select(x => x.Id).ToList();
            if (ids.Count == 0) return 0;
            return _db.Update<RecordFile>().Where(x => ids.Contains(x.Id) && x.Deleted == true && x.Undo == true)
                .Set(x => x.Undo, false).Set(x => x.UpdateTime, DateTime.Now).ExecuteAffrows();
        }

        /// <summary>检查录制回调元数据；异常只表示需要核实，不能据此删除视频文件。</summary>
        /// <param name="duration">回调中的录制秒数。</param>
        /// <param name="size">回调中的文件字节数。</param>
        /// <returns>仅非空、正数时长和正常范围的字节数可以写入有效录像索引。</returns>
        public static bool ValidRecordingMetadata(decimal? duration, long? size) =>
            duration.HasValue && duration > 0 && duration <= int.MaxValue &&
            size.HasValue && size > 0 && size <= 103881427200;

        /// <summary>读取同一节点、同一路径的全部索引，防止旧删除标记或其他节点被混淆。</summary>
        private List<RecordFile> Find(string mediaServerId, string path) => _db.Select<RecordFile>()
            .Where(x => x.MediaServerId == mediaServerId && x.VideoPath == path).ToList();
    }
}
