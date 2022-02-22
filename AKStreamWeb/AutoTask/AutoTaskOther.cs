using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibLogger;

namespace AKStreamWeb.AutoTask
{
    public class AutoTaskOther
    {
        private long count = 0;

        public AutoTaskOther()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    run();
                }
                catch
                {
                }
            })).Start();
        }

        private void run()
        {
            while (true)
            {
                if (count > 90000000000)
                {
                    count = 0;
                }

                count++;
                try
                {
                    if (count % 5 ==  0)//5秒清理一次为null的MediaServer
                    {
                        lock (Common.MediaServerLockObj)
                        {
                            for (int i = Common.MediaServerList.Count - 1; i >= 0; i--)
                            {
                                if (!Common.MediaServerList[i].IsKeeperRunning && !Common.MediaServerList[i].IsMediaServerRunning)
                                {
                                    Common.MediaServerList[i] = null;
                                }    
                            }
                           UtilsHelper.RemoveNull(Common.MediaServerList);
                          
                        }
                    }
                    if (count % 3600 == 0) //3600秒一次
                    {
                        doDeleteFor24HourAgo(); //删除24小时前被软删除的过期失效的文件
                        doDeleteLostRecored();//删除失效的数据库录像数据
                        GCommon.VideoChannelRecordInfo.RemoveAll(x => x.Expires < DateTime.Now);
                    }

                    if (count % 3600 == 0) //3600秒一次
                    {
                        foreach (var mediaServer in Common.MediaServerList)
                        {
                            if (mediaServer != null && mediaServer.KeeperWebApi != null && mediaServer.IsKeeperRunning)
                            {
                                mediaServer.KeeperWebApi.CleanUpEmptyDir(out _);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                   GCommon.Logger.Debug(ex.Message);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 删除已经过了期限的失效记录（录像文件已经被删除，数据库记录并没有实际删除，这个方法是为了删除已经删除但数据库中还存在的数据记录）
        /// 为了优化数据库性能，将超过有效期的失效数据删除，如果配置文件中DeletedRecordsExpiredDays字段<=0则不做处理
        /// </summary>
        private void doDeleteLostRecored()
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

        private void doDeleteFor24HourAgo()
        {
            try
            {
                List<RecordFile> retList = null!;
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