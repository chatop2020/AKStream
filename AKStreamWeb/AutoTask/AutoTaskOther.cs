using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs.DBModels;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.AutoTask
{
    public class AutoTaskOther
    {
        private DateTime _nullMediaServerCheckTick = DateTime.Now;
        private DateTime _deleteOldVideoTick = DateTime.Now;
        private DateTime _cleanUpEmptyDirTick = DateTime.Now;
        private DateTime _mediaListCleanTick = DateTime.Now;
        private Thread RunDeleteOrphanDataHandle = null;
        private bool canSuspend = true;
        private bool setSuspend = false;
        private bool oldState = false;

        public AutoTaskOther()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    Run();
                }
                catch
                {
                }
            })).Start();

            ThreadStart entry = new ThreadStart(RunDeleteOrphanData);
            RunDeleteOrphanDataHandle = new Thread(entry);
            RunDeleteOrphanDataHandle.Start();

            new Thread(new ThreadStart(delegate
            {
                try
                {
                    CheckIdleRun();
                }
                catch
                {
                }
            })).Start();
        }


        /// <summary>
        /// 检测资源是否空闲，cpu占用率低于35%时认为是空闲状态
        /// </summary>
        private void CheckIdleRun()
        {
            ResponseStruct rs = null;
            var oldStatus = false;

            while (true)
            {
                try
                {
                    if (Common.WebPerformanceInfo != null && Common.WebPerformanceInfo.CpuLoad < 35f &&
                        RunDeleteOrphanDataHandle != null)
                    {
                        setSuspend = false;
                        // RunDeleteOrphanDataHandle.Resume();   //linux平台不支持此方法
                    }
                    else
                    {
                        while (!canSuspend)
                        {
                            Thread.Sleep(100);
                        }

                        if (RunDeleteOrphanDataHandle != null)
                        {
                            setSuspend = true;
                            //  RunDeleteOrphanDataHandle.Suspend(); //linux平台不支持此方法
                        }
                    }

                    if (oldState != setSuspend)
                    {
                        oldState = setSuspend;
                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->当前双向清理孤立数据功能状态为：{(setSuspend ? "挂起" : "运行")}->CPUUsage:{Common.WebPerformanceInfo.CpuLoad}%");
                    }
                }
                catch (Exception ex)
                {
                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->发生异常情况：{ex.Message}->{ex.StackTrace}");
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 当相对空闲时双向清理孤立数据，（双向孤立数据：mysql中存在而磁盘不存在，磁盘存在而mysql不存在的数据）
        /// </summary>
        private void RunDeleteOrphanData()
        {
            canSuspend = true;
            int i = 0;
            DeleteOrphanDataDir.DataDir dir;
            while (true)
            {
                i++;
                dir = UtilsHelper.IsOdd(i) ? (DeleteOrphanDataDir.DataDir.MySql) : (DeleteOrphanDataDir.DataDir.Disk);
                switch (dir)
                {
                    case DeleteOrphanDataDir.DataDir.Disk:
                        try
                        {
                            List<string> fileRecordDropList = new List<string>();
                            foreach (var mediaServer in Common.MediaServerList)
                            {
                                canSuspend = false;
                                if (mediaServer != null && mediaServer.IsKeeperRunning)
                                {
                                    canSuspend = true;
                                    while (setSuspend)
                                    {
                                        Thread.Sleep(1000);
                                    }

                                    if (mediaServer.RecordPathList != null && mediaServer.RecordPathList.Count > 0)
                                    {
                                        foreach (var recordPath in mediaServer.RecordPathList)
                                        {
                                            var dirPath = recordPath.Value;
                                            if (Directory.Exists(dirPath))
                                            {
                                                DirectoryInfo di = new DirectoryInfo(dirPath);
                                                canSuspend = false;
                                                FileInfo[] fis = di.GetFiles("*.mp4", SearchOption.AllDirectories);
                                                canSuspend = true;
                                                while (setSuspend)
                                                {
                                                    Thread.Sleep(1000);
                                                }

                                                if (fis != null && fis.Length > 0)
                                                {
                                                    foreach (var f in fis)
                                                    {
                                                        if (f.Exists && !f.Name.StartsWith(".") &&
                                                            !f.FullName.StartsWith(mediaServer.CutMergeFilePath))
                                                        {
                                                            canSuspend = false;
                                                            if (Common.IsDebug)
                                                            {
                                                                var sql = ORMHelper.Db.Select<RecordFile>()
                                                                    .Where(x => x.VideoPath.Equals(f.FullName.Trim()))
                                                                    .ToSql();

                                                                GCommon.Logger.Debug(
                                                                    $"[{Common.LoggerHead}]->RunDeleteOrphanData->执行SQL:->{sql}");
                                                            }

                                                            var exists = ORMHelper.Db.Select<RecordFile>()
                                                                .Where(x => x.VideoPath.Equals(f.FullName.Trim()))
                                                                .ToOne();
                                                            canSuspend = true;
                                                            while (setSuspend)
                                                            {
                                                                Thread.Sleep(1000);
                                                            }

                                                            if (exists == null || (exists.Deleted == true &&
                                                                    exists.Undo == false)) //记录不存在，或者被硬删除
                                                            {
                                                                fileRecordDropList.Add(f.FullName);
                                                            }
                                                        }

                                                        Thread.Sleep(200);
                                                    }

                                                    if (fileRecordDropList != null && fileRecordDropList.Count > 0)
                                                    {
                                                        foreach (var file in fileRecordDropList)
                                                        {
                                                            canSuspend = false;
                                                            var s = mediaServer.KeeperWebApi.DeleteFile(out _, file);
                                                            GCommon.Logger.Debug(
                                                                $"[{Common.LoggerHead}]->双向清理孤立数据->Disk->{file}->{(s ? "成功" : "失败")}");
                                                            canSuspend = true;
                                                            while (setSuspend)
                                                            {
                                                                Thread.Sleep(1000);
                                                            }

                                                            Thread.Sleep(200);
                                                        }
                                                    }
                                                }
                                            }

                                            Thread.Sleep(200);
                                        }
                                    }
                                }

                                canSuspend = true;
                                while (setSuspend)
                                {
                                    Thread.Sleep(1000);
                                }

                                Thread.Sleep(200);
                            }
                        }
                        catch (Exception ex)
                        {
                            canSuspend = true;
                            while (setSuspend)
                            {
                                Thread.Sleep(1000);
                            }

                            GCommon.Logger.Debug($"{ex.Message}\r\n{ex.StackTrace}");
                        }

                        break;
                    case DeleteOrphanDataDir.DataDir.MySql:
                        try
                        {
                            List<long> mysqlRecordDropList = new List<long>();
                            canSuspend = false;
                            var recordList = ORMHelper.Db.Select<RecordFile>().Where(x => x.Deleted == false).ToList();
                            canSuspend = true;
                            while (setSuspend)
                            {
                                Thread.Sleep(1000);
                            }

                            foreach (var record in recordList)
                            {
                                if (record != null)
                                {
                                    var filePath = record.VideoPath;
                                    var mediaserver2 = Common.MediaServerList.FindLast(x =>
                                        x.MediaServerId.Equals(record.MediaServerId));
                                    if (mediaserver2 != null && mediaserver2.IsKeeperRunning &&
                                        !string.IsNullOrEmpty(filePath))
                                    {
                                        canSuspend = false;

                                        var exists = mediaserver2.KeeperWebApi.FileExists(out _, filePath);
                                        Thread.Sleep(150);
                                        canSuspend = true;
                                        while (setSuspend)
                                        {
                                            Thread.Sleep(1000);
                                        }

                                        if (!exists)
                                        {
                                            mysqlRecordDropList.Add(record.Id);
                                        }
                                    }
                                }

                                Thread.Sleep(200);
                            }

                            if (mysqlRecordDropList != null && mysqlRecordDropList.Count > 0)
                            {
                                foreach (var id in mysqlRecordDropList)
                                {
                                    canSuspend = false;
                                    if (Common.IsDebug)
                                    {
                                        var sql = ORMHelper.Db.Delete<RecordFile>().Where(x => x.Id.Equals(id))
                                            .ToSql();

                                        GCommon.Logger.Debug(
                                            $"[{Common.LoggerHead}]->RunDeleteOrphanData->执行SQL:->{sql}");
                                    }

                                    var s2 = ORMHelper.Db.Delete<RecordFile>().Where(x => x.Id.Equals(id))
                                        .ExecuteAffrows();
                                    GCommon.Logger.Debug(
                                        $"[{Common.LoggerHead}]->双向清理孤立数据->MySQL->{id}->{(s2 > 0 ? "成功" : "失败")}");
                                    canSuspend = true;
                                    while (setSuspend)
                                    {
                                        Thread.Sleep(1000);
                                    }

                                    Thread.Sleep(200);
                                }
                            }

                            canSuspend = true;
                            while (setSuspend)
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            canSuspend = true;
                            while (setSuspend)
                            {
                                Thread.Sleep(1000);
                            }

                            GCommon.Logger.Debug($"{ex.Message}\r\n{ex.StackTrace}");
                        }

                        break;
                }

                canSuspend = true;
                while (setSuspend)
                {
                    Thread.Sleep(1000);
                }

                Thread.Sleep(10000);
            }
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    if ((DateTime.Now - _nullMediaServerCheckTick).TotalMilliseconds > -5000) //5秒清理一次为null的MediaServer
                    {
                        lock (Common.MediaServerLockObj)
                        {
                            for (int i = Common.MediaServerList.Count - 1; i >= 0; i--)
                            {
                                if (!Common.MediaServerList[i].IsKeeperRunning &&
                                    !Common.MediaServerList[i].IsMediaServerRunning)
                                {
                                    Common.MediaServerList[i] = null;
                                }
                            }

                            UtilsHelper.RemoveNull(Common.MediaServerList);
                        }

                        _nullMediaServerCheckTick = DateTime.Now;
                    }

                    if ((DateTime.Now - _deleteOldVideoTick).TotalMilliseconds >= 3600000) //3600秒一次
                    {
                        DoDeleteFor24HourAgo(); //删除24小时前被软删除的过期失效的文件
                        DoDeleteLostRecored(); //删除失效的数据库录像数据
                        GCommon.VideoChannelRecordInfo.RemoveAll(x => x.Expires < DateTime.Now);
                        _deleteOldVideoTick = DateTime.Now;
                    }


                    if ((DateTime.Now - _cleanUpEmptyDirTick).TotalMilliseconds >= 360000) //3600秒一次
                    {
                        foreach (var mediaServer in Common.MediaServerList)
                        {
                            if (mediaServer != null && mediaServer.KeeperWebApi != null && mediaServer.IsKeeperRunning)
                            {
                                mediaServer.KeeperWebApi.CleanUpEmptyDir(out _);
                            }
                        }

                        _cleanUpEmptyDirTick = DateTime.Now;
                    }


                    if ((DateTime.Now - _mediaListCleanTick).TotalMilliseconds >=
                        30000) //30秒请求一次zlm的流列表,查看自己的列表中是否存在流列表中不存的流
                    {
                        try
                        {
                            lock (GCommon.Ldb.LiteDBLockObj)
                            {
                                var selfMediaList = GCommon.Ldb.VideoOnlineInfo.FindAll().ToList();
                                if (selfMediaList != null && selfMediaList.Count() > 0)
                                {
                                    foreach (var mediaServer in Common.MediaServerList)
                                    {
                                        if (mediaServer != null && mediaServer.KeeperWebApi != null &&
                                            mediaServer.IsKeeperRunning)
                                        {
                                            ResponseStruct rs = null;
                                            var mediaList =
                                                mediaServer.WebApiHelper.GetMediaList(new ReqZLMediaKitGetMediaList(),
                                                    out rs);
                                            if (mediaList.Data == null && mediaList != null && mediaList.Code == 0 &&
                                                rs.Code.Equals(ErrorNumber.None))
                                            {
                                                GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                                                    x.MediaServerId.Equals(mediaServer.MediaServerId));
                                                GCommon.Logger.Info(
                                                    $"[{Common.LoggerHead}]->30秒任务->发现[{mediaServer.MediaServerId}]流媒体服务器中不存在任何，删除VideoOnlineInfo中所有关于[{mediaServer.MediaServerId}]的流数据...");
                                            }
                                            else
                                            {
                                                if (mediaList != null && mediaList.Code == 0 &&
                                                    rs.Code.Equals(ErrorNumber.None) && mediaList.Data != null &&
                                                    mediaList.Data.Count > 0)
                                                {
                                                    foreach (var media in selfMediaList)
                                                    {
                                                        if (media != null && mediaList.Data.FindLast(x =>
                                                                x.Stream.Equals(media.MainId) &&
                                                                x.App.Equals(media.App)) ==
                                                            null &&
                                                            media.MediaServerStreamInfo.MediaServerId.Equals(mediaServer
                                                                .MediaServerId))
                                                        {
                                                            if (media.LostCount >= media.TimesToDelete)
                                                            {
                                                                GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                                                                    x.MainId.Equals(media.MainId) &&
                                                                    x.App.Equals(media.App) &&
                                                                    x.MediaServerId.Equals(mediaServer.MediaServerId));
                                                                GCommon.Logger.Info(
                                                                    $"[{Common.LoggerHead}]->30秒任务->发现[{mediaServer.MediaServerId}][{media.App}][{media.MainId}]->流在流媒体服务器中不存在，删除VideoOnlineInfo中相应流数据...");
                                                            }
                                                            else
                                                            {
                                                                media.LostCount++;
                                                                GCommon.Logger.Info(
                                                                    $"[{Common.LoggerHead}]->30秒任务->发现[{mediaServer.MediaServerId}][{media.App}][{media.MainId}]->流在流媒体服务器中不存在，检测次数{media.LostCount}/{media.TimesToDelete}...");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (media.LostCount > 0)
                                                            {
                                                                media.LostCount--;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (mediaList != null && mediaList.Code == 0 &&
                                                         mediaList.Data.Count <= 0)
                                                {
                                                    GCommon.Ldb.VideoOnlineInfo.DeleteMany(x =>
                                                        x.MediaServerId.Equals(mediaServer.MediaServerId));
                                                    GCommon.Logger.Info(
                                                        $"[{Common.LoggerHead}]->30秒任务->发现[{mediaServer.MediaServerId}]流媒体服务器中不存在任何，删除VideoOnlineInfo中所有关于[{mediaServer.MediaServerId}]的流数据...");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GCommon.Logger.Debug($"{ex.Message}\r\n{ex.StackTrace}");
                        }

                        _mediaListCleanTick = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    GCommon.Logger.Debug($"{ex.Message}\r\n{ex.StackTrace}");
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 删除已经过了期限的失效记录（录像文件已经被删除，数据库记录并没有实际删除，这个方法是为了删除已经删除但数据库中还存在的数据记录）
        /// 为了优化数据库性能，将超过有效期的失效数据删除，如果配置文件中DeletedRecordsExpiredDays字段<=0则不做处理
        /// </summary>
        private void DoDeleteLostRecored()
        {
            try
            {
                if (Common.AkStreamWebConfig.DeletedRecordsExpiredDays > 0)
                {
                    #region debug sql output

                    if (Common.IsDebug)
                    {
                        var sql = ORMHelper.Db.Delete<RecordFile>()
                            .Where(x => x.Deleted == true)
                            .Where(x => x.Undo == false)
                            .Where(x => x.UpdateTime <=
                                        DateTime.Now.AddDays(-Common.AkStreamWebConfig.DeletedRecordsExpiredDays))
                            .ToSql();

                        GCommon.Logger.Debug(
                            $"[{Common.LoggerHead}]->DoDeleteLostRecored->执行SQL:->{sql}");
                    }

                    #endregion

                    ORMHelper.Db.Delete<RecordFile>()
                        .Where(x => x.Deleted == true)
                        .Where(x => x.Undo == false)
                        .Where(x => x.UpdateTime <=
                                    DateTime.Now.AddDays(-Common.AkStreamWebConfig.DeletedRecordsExpiredDays))
                        .ExecuteAffrows();
                }
            }
            catch
            {
            }
        }

        private void DoDeleteFor24HourAgo()
        {
            try
            {
                List<RecordFile> retList = null!;

                #region debug sql output

                if (Common.IsDebug)
                {
                    var sql = ORMHelper.Db.Select<RecordFile>()
                        .Where(x => x.Deleted == true)
                        .Where(x => x.Undo == true)
                        .Where(x => ((DateTime)x.UpdateTime!).AddHours(24) <= DateTime.Now).ToSql();

                    GCommon.Logger.Debug(
                        $"[{Common.LoggerHead}]->DoDeleteFor24HourAgo->执行SQL:->{sql}");
                }

                #endregion

                retList = ORMHelper.Db.Select<RecordFile>()
                    .Where(x => x.Deleted == true)
                    .Where(x => x.Undo == true)
                    .Where(x => ((DateTime)x.UpdateTime!).AddHours(24) <= DateTime.Now)
                    .ToList();

                if (retList != null && retList.Count > 0)
                {
                    var deleteFileList = retList.Select(x => x.VideoPath).ToList();
                    var deleteFileIdList = retList.Select(x => x.Id).ToList();

                    var mediaServer =
                        Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retList[0].MediaServerId));
                    if (mediaServer != null && mediaServer.IsKeeperRunning)
                    {
                        ResponseStruct rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.None,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                        };
                        var delRet =
                            AKStreamKeeperService.DeleteFileList(mediaServer.MediaServerId, deleteFileList, out rs);
                        // var delRet = mediaServer.KeeperWebApi.DeleteFileList(out _, deleteFileList);

                        if (rs.Code == ErrorNumber.MediaServer_DiskExcept)
                        {
                            GCommon.Logger.Warn(
                                $"[{Common.LoggerHead}]->删除24小时前被软删除记录文件时发生异常->{JsonHelper.ToJson(rs)}");
                            return;
                        }

                        #region debug sql output

                        if (Common.IsDebug)
                        {
                            var sql = ORMHelper.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                                .Set(x => x.Undo, false)
                                .Where(x => deleteFileIdList.Contains(x.Id)).ToSql();

                            GCommon.Logger.Debug(
                                $"[{Common.LoggerHead}]->DoDeleteFor24HourAgo->执行SQL:->{sql}");
                        }

                        #endregion

                        var a = ORMHelper.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                            .Set(x => x.Undo, false)
                            .Where(x => deleteFileIdList.Contains(x.Id)).ExecuteAffrows();
                    }
                }
            }
            catch (Exception ex)
            {
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除被软删除记录文件时发生异常->{ex.Message}->{ex.StackTrace}");
            }
        }
    }
}