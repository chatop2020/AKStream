using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;

namespace AKStreamWeb.AutoTask
{
    public class AutoTaskOther
    {
        private  DateTime _nullMediaServerCheckTick=DateTime.Now;
        private DateTime _deleteOldVideoTick=DateTime.Now;
        private DateTime _cleanUpEmptyDirTick=DateTime.Now;
        private DateTime _mediaListCleanTick=DateTime.Now;
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
        }

        private void Run()
        {
            while (true)
            {
             
                try
                {
                    if((DateTime.Now-_nullMediaServerCheckTick).TotalMilliseconds>-5000)//5秒清理一次为null的MediaServer
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
                        _nullMediaServerCheckTick=DateTime.Now;
                        
                    }
                    
                    if((DateTime.Now-_deleteOldVideoTick).TotalMilliseconds>=3600000)//3600秒一次
                    {
                        DoDeleteFor24HourAgo(); //删除24小时前被软删除的过期失效的文件
                        DoDeleteLostRecored(); //删除失效的数据库录像数据
                        GCommon.VideoChannelRecordInfo.RemoveAll(x => x.Expires < DateTime.Now);
                        _deleteOldVideoTick=DateTime.Now;
                        
                    }

                   
                    if((DateTime.Now-_cleanUpEmptyDirTick).TotalMilliseconds>=360000)//3600秒一次
                    {
                        foreach (var mediaServer in Common.MediaServerList)
                        {
                            if (mediaServer != null && mediaServer.KeeperWebApi != null && mediaServer.IsKeeperRunning)
                            {
                                mediaServer.KeeperWebApi.CleanUpEmptyDir(out _);
                            }
                        }
                        _cleanUpEmptyDirTick=DateTime.Now;
                    }

                    
                    if((DateTime.Now-_mediaListCleanTick).TotalMilliseconds>=30000)//30秒请求一次zlm的流列表,查看自己的列表中是否存在流列表中不存的流
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
                                                mediaServer.WebApiHelper.GetMediaList(new ResZLMediaKitGetMediaList(),
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

                        _mediaListCleanTick=DateTime.Now;
                        
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
                retList = ORMHelper.Db.Select<RecordFile>()
                    .Where(x => x.Deleted == true)
                    .Where(x => x.Undo == true)
                    .Where(x => ((DateTime) x.UpdateTime!).AddHours(24) <= DateTime.Now)
                    .ToList();

                if (retList != null && retList.Count > 0)
                {
                    var deleteFileList = retList.Select(x => x.VideoPath).ToList();
                    var deleteFileIdList = retList.Select(x => x.Id).ToList();

                    var mediaServer =
                        Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retList[0].MediaServerId));
                    if (mediaServer != null && mediaServer.IsKeeperRunning)
                    {
                        var delRet = mediaServer.KeeperWebApi.DeleteFileList(out _, deleteFileList);

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