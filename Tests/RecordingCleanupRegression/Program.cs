using AKStreamKeeper.Services;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebResponse.AKStreamKeeper;

/// <summary>使用临时录像目录及真实 SQLite 索引，复现短暂断盘、索引丢失和双向清理的组合。</summary>
internal static class Program
{
    private static int _checks;

    /// <summary>不读取现场配置，不连接生产 MySQL；任一安全条件失败就以非零退出码结束。</summary>
    public static void Main()
    {
        var root = Path.Combine(Path.GetTempPath(), "akstream-recording-safety-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        Environment.CurrentDirectory = root;
        Directory.CreateDirectory("Config");
        File.WriteAllText("Config/logconfig.xml", "<log4net><root><level value=\"OFF\" /></root></log4net>");
        GCommon.InitLogger();
        // 不启动 Keeper 历史静态初始化带来的前台性能采样，保证隔离验证可正常退出。
        typeof(LibSystemInfo.SystemInfo).GetField("_abort", System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.NonPublic)!.SetValue(null, true);
        SQLitePCL.Batteries_V2.Init();
        using var db = new FreeSql.FreeSqlBuilder().UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=:memory:",
            Type.GetType("FreeSql.Sqlite.SqliteProvider`1, FreeSql.Provider.SqliteCore")!).Build();
        db.CodeFirst.SyncStructure<RecordFile>();
        var cleaner = new RecordingOrphanCleanup(db);
        var disk = Path.Combine(root, "disk");
        Directory.CreateDirectory(disk);
        var path = Path.Combine(disk, "record.mp4");
        File.WriteAllText(path, "original recording bytes");
        var row = Record(path);
        db.Insert(row).ExecuteAffrows();
        var indexed = db.Select<RecordFile>().Where(x => x.VideoPath == path).First();
        var exists = ApiService.FileExists(path, out var status);
        Check(RecordingOrphanCleanup.ObserveFile(exists, status) == RecordingFileObservation.Present, "健康磁盘录像存在");

        // 用真实目录移走/恢复模拟设备暂时不可访问，文件内容仍在原存储中。
        var disconnected = disk + "-offline";
        Directory.Move(disk, disconnected);
        exists = ApiService.FileExists(path, out status);
        Check(RecordingOrphanCleanup.ObserveFile(exists, status) != RecordingFileObservation.Present, "断盘时观测为缺失或不可核实");
        Check(db.Select<RecordFile>().Where(x => x.Id == indexed.Id && x.Deleted == false).Any(), "文件暂不可访问不能删除或标记删除索引");
        Directory.Move(disconnected, disk);
        var deletes = 0;
        bool Delete(string file) { deletes++; File.Delete(file); return true; }
        var outcome = cleaner.InspectDiskFile("node-a", path, true, Delete);
        Check(outcome == RecordingDiskOutcome.Indexed && deletes == 0 && File.Exists(path), "恢复可读后仍保留原录像和索引");

        // 即使数据库记录在修复前已经丢失，恢复后的文件也不能被第二方向扫描删除。
        db.Delete<RecordFile>().Where(x => x.Id == indexed.Id).ExecuteAffrows();
        outcome = cleaner.InspectDiskFile("node-a", path, true, Delete);
        Check(outcome == RecordingDiskOutcome.UnindexedPreserved && deletes == 0 && File.Exists(path), "已丢失索引的原始文件仍被保留");
        Check(File.ReadAllText(path) == "original recording bytes", "断盘与恢复组合不改变录像内容");

        var missing = Path.Combine(disk, "missing.mp4");
        db.Insert(Record(missing)).ExecuteAffrows();
        exists = ApiService.FileExists(missing, out status);
        Check(RecordingOrphanCleanup.ObserveFile(exists, status) == RecordingFileObservation.Missing, "真正缺失与访问错误分开报告");
        Check(db.Select<RecordFile>().Where(x => x.VideoPath == missing && x.Deleted == false).Any(), "确认缺失也只告警保留索引供人工核对");
        foreach (var exception in new Exception[] { new IOException("test I/O failure"), new UnauthorizedAccessException("test access denied") })
        {
            exists = RecordingFileProbe.Exists(path, out status, _ => throw exception);
            Check(RecordingOrphanCleanup.ObserveFile(exists, status) == RecordingFileObservation.Unavailable,
                "I/O 或权限错误不能伪装成文件不存在");
        }
        Check(RecordingOrphanCleanup.ObserveFile(false, null!) == RecordingFileObservation.Unavailable, "空检查结果不能当成文件丢失");

        db.Insert(Record(path, deleted: true, undo: false)).ExecuteAffrows();
        Check(cleaner.InspectDiskFile("node-b", path, true, Delete) == RecordingDiskOutcome.UnindexedPreserved,
            "同一路径的其他节点记录不能授权删除本节点文件");
        Check(cleaner.InspectDiskFile("node-a", path, false, Delete) == RecordingDiskOutcome.DeletionDeferred && deletes == 0,
            "明确硬删除在磁盘健康未知时也延期");
        Check(cleaner.InspectDiskFile("node-a", path, true, Delete) == RecordingDiskOutcome.Deleted && deletes == 1 && !File.Exists(path),
            "正常明确硬删除仍可执行");
        var recoverable = Path.Combine(disk, "soft-deleted.mp4");
        File.WriteAllText(recoverable, "recoverable");
        db.Insert(Record(recoverable, deleted: true, undo: true)).ExecuteAffrows();
        Check(cleaner.InspectDiskFile("node-a", recoverable, true, Delete) == RecordingDiskOutcome.Indexed && File.Exists(recoverable),
            "仍可恢复的软删除文件不能被孤立扫描删除");
        var reused = Path.Combine(disk, "reused-name.mp4");
        File.WriteAllText(reused, "new recording at reused path");
        db.Insert(Record(reused, deleted: true, undo: false)).ExecuteAffrows();
        db.Insert(Record(reused)).ExecuteAffrows();
        Check(cleaner.InspectDiskFile("node-a", reused, true, Delete) == RecordingDiskOutcome.Indexed && File.Exists(reused),
            "旧删除标记不能删除同一路径已有有效索引的新录像");

        var pending = Path.Combine(disk, "pending-delete.mp4");
        File.WriteAllText(pending, "pending");
        db.Insert(Record(pending, deleted: true, undo: true)).ExecuteAffrows();
        var soft = db.Select<RecordFile>().Where(x => x.Deleted == true && x.Undo == true).ToList();
        Check(cleaner.FinalizeSoftDeletes(soft, null!, Success()) == 0, "删除接口无响应不能确认整批删除");
        Check(cleaner.FinalizeSoftDeletes(soft, new ResKeeperDeleteFileList(), new ResponseStruct { Code = ErrorNumber.MediaServer_DiskExcept }) == 0,
            "磁盘错误不能推进删除队列");
        var completed = cleaner.FinalizeSoftDeletes(soft, new ResKeeperDeleteFileList { PathList = new List<string> { pending } }, Success());
        Check(completed == 1 && db.Select<RecordFile>().Where(x => x.VideoPath == pending && x.Undo == true).Any(),
            "批量部分失败只确认实际完成项");
        var batch = ApiService.DeleteFileList(new List<string> { Path.Combine(root, "unavailable-dir", "file.mp4") }, out status);
        Check(batch.PathList!.Count == 1, "不可访问目录中的删除不能伪装成批量成功");

        var states = new Dictionary<string, int> { [disk] = -2, [Path.Combine(disk, "healthy")] = 0 };
        Check(RecordingOrphanCleanup.StorageStatus(path, states) == -2, "匹配录像实际所属故障磁盘");
        Check(RecordingOrphanCleanup.StorageStatus(Path.Combine(disk, "healthy", "file.mp4"), states) == 0, "最长根目录优先，其他磁盘故障不串用");
        Check(RecordingOrphanCleanup.StorageStatus(Path.Combine(root, "disk-other", "file.mp4"), states) == null, "相似前缀不能误认存储根目录");
        Check(RecordingOrphanCleanup.StorageStatus(path, new Dictionary<string, int>()) == null, "空磁盘状态是未知而非健康");
        foreach (var metadata in new[] { (Duration: (decimal?)0m, Size: 3761123821390083890L), (Duration: (decimal?)null, Size: 100L), (Duration: (decimal?)1m, Size: -1L) })
        {
            Check(!RecordingOrphanCleanup.ValidRecordingMetadata(metadata.Duration, metadata.Size), "异常录制元数据需要核实");
        }
        Check(RecordingOrphanCleanup.ValidRecordingMetadata(120, 1048576), "正常录像元数据仍可接受");
        var unindexed = Path.Combine(disk, "bad-metadata.mp4");
        File.WriteAllText(unindexed, "preserve despite invalid callback metadata");
        Check(cleaner.InspectDiskFile("node-a", unindexed, true, Delete) == RecordingDiskOutcome.UnindexedPreserved && File.Exists(unindexed),
            "异常回调未生成索引也不能导致录像被扫描删除");
        db.Aop.CurdBefore += (_, _) => throw new IOException("test database unavailable");
        var rejected = false;
        try { cleaner.InspectDiskFile("node-a", unindexed, true, Delete); } catch { rejected = true; }
        Check(rejected && File.Exists(unindexed), "数据库查询失败时不调用文件删除");
        Console.WriteLine($"PASS {_checks} checks; isolated_directory={root}");
    }

    /// <summary>建立最小录像索引，删除标志只由测试场景显式指定。</summary>
    private static RecordFile Record(string path, bool deleted = false, bool undo = false) => new RecordFile
    {
        MainId = "stream", MediaServerId = "node-a", MediaServerIp = "127.0.0.1", VideoPath = path,
        Deleted = deleted, Undo = undo, CreateTime = DateTime.Now, UpdateTime = DateTime.Now
    };

    /// <summary>创建明确成功的内部访问结果。</summary>
    private static ResponseStruct Success() => new ResponseStruct { Code = ErrorNumber.None };

    /// <summary>检查实际文件和索引结果，任何失败立即停止并返回非零退出码。</summary>
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + message);
        _checks++;
        Console.WriteLine("PASS: " + message);
    }
}
