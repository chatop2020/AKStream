using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;
using LibLogger;

namespace AKStreamWeb.AutoTask
{
    public class AutoRecord
    {

        private void doDeleteFor24HourAgo()
        {
            try
            {
                List<RecordFile> retList = null!;
                retList = ORMHelper.Db.Select<RecordFile>()
                    .Where(x => x.Deleted == true)
                    .Where(x=>x.Undo==true)
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
                        var delRet = mediaServer.KeeperWebApi.DeleteFileList( out _ ,deleteFileList);

                        foreach (var ret in retList)
                        {
                            var o=delRet.PathList.FindLast(x => x.Equals(ret.VideoPath));
                            if (string.IsNullOrEmpty(o))
                            {
                                var o2 = deleteFileIdList.FindLast(x => x.Equals(ret.Id));
                                if (o2 != null && o2 > 0)
                                {
                                    deleteFileIdList.Remove(o2);
                                }
                            }
                        }
                        if (deleteFileIdList!=null && deleteFileIdList.Count>0)
                        {
                            var a = ORMHelper.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                                .Set(x => x.Undo, false)
                                .Where(x => deleteFileIdList.Contains(x.Id)).ExecuteAffrows();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"[{Common.LoggerHead}]->删除被软件删除记录文件时发生异常->{ex.Message}->{ex.StackTrace}");
            }
        }
        private void KeepRecord()
        {
            while (true)
            {
                doDeleteFor24HourAgo();//删除24小时前被软删除的过期失效的文件
                var recordPlanList = RecordPlanService.GetRecordPlanList("", out ResponseStruct rs);
                var videoChannelList = ORMHelper.Db.Select<VideoChannel>().Where(x => x.Enabled.Equals(true))
                    .Where(x => !string.IsNullOrEmpty(x.RecordPlanName))
                    .Where(x => x.AutoRecord.Equals(true)).ToList();
                if (!rs.Code.Equals(ErrorNumber.None) || recordPlanList==null || recordPlanList.Count<=0)
                {
                    continue;
                }

                try
                {
                    foreach (var obj in Common.VideoChannelMediaInfos)
                    {
                        if (obj != null)
                        {
                            var videoChannel = videoChannelList.FindLast(x => x.MainId.Equals(obj.MainId));
                            if (videoChannel != null)
                            {
                                //启用了自动录制
                                var recordPlan =
                                    recordPlanList.FindLast(x => x.Name.Equals(videoChannel.RecordPlanName));
                                if (recordPlan != null)
                                {
                                    //说明绑定了录制模板
                                    
                                }
                            }

                        }
                    }
                }
                catch
                {
                    //
                }

                Thread.Sleep(1000);
            }
        }

        
     

        public AutoRecord()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepRecord();
                }
                catch
                {
                  
                }
            })).Start();
        }
    }
}