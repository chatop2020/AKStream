using System.Threading;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibLogger;

namespace AKStreamWeb.AutoTask
{
    public class AutoLive
    {
        public AutoLive()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepLive();
                }
                catch
                {
                }
            })).Start();
        }

        private void KeepLive()
        {
            while (true)
            {
                try
                {
                    
                    var dbRet = ORMHelper.Db.Select<VideoChannel>().ToList();
                    if (dbRet != null && dbRet.Count > 0)
                    {
                        foreach (var obj in dbRet)
                        {
                            var listRet = GCommon.Ldb.VideoOnlineInfo.FindOne(x =>
                                x.MainId.Equals(obj.MainId) && x.MediaServerId.Equals(obj.MediaServerId));
                            if (obj != null && obj.AutoVideo.Equals(true) && obj.NoPlayerBreak.Equals(false) &&
                                obj.Enabled.Equals(true))
                            {
                                if (listRet == null)
                                {
                                    var mediaServer = Common.MediaServerList.FindLast(x =>
                                        x.MediaServerId.Equals(obj.MediaServerId));
                                    if (mediaServer != null && mediaServer.IsKeeperRunning &&
                                        mediaServer.IsMediaServerRunning)
                                    {
                                        var streamLiveRet = MediaServerService.StreamLive(obj.MediaServerId, obj.MainId,
                                            out ResponseStruct rs);
                                        if (!rs.Code.Equals(ErrorNumber.None) || streamLiveRet == null)
                                        {
                                             GCommon.Logger.Warn(
                                                $"[{Common.LoggerHead}]->自动推流失败->{obj.MediaServerId}->{obj.MainId}");
                                        }
                                        else
                                        {
                                             GCommon.Logger.Info(
                                                $"[{Common.LoggerHead}]->自动推流成功->{obj.MediaServerId}->{obj.MainId}");
                                        }
                                    }
                                }
                            }
                            else if (obj != null && obj.Enabled.Equals(false))
                            {
                                if (listRet != null)
                                {
                                    var mediaServer = Common.MediaServerList.FindLast(x =>
                                        x.MediaServerId.Equals(obj.MediaServerId));
                                    if (mediaServer != null && mediaServer.IsKeeperRunning &&
                                        mediaServer.IsMediaServerRunning)
                                    {
                                        var streamLiveRet = MediaServerService.StreamStop(obj.MediaServerId, obj.MainId,
                                            out ResponseStruct rs);
                                        if (!rs.Code.Equals(ErrorNumber.None) || streamLiveRet == null)
                                        {
                                             GCommon.Logger.Warn(
                                                $"[{Common.LoggerHead}]->自动结束推流失败->{obj.MediaServerId}->{obj.MainId}");
                                        }
                                        else
                                        {
                                             GCommon.Logger.Info(
                                                $"[{Common.LoggerHead}]->自动结束推流成功->{obj.MediaServerId}->{obj.MainId}");
                                        }
                                    }
                                }
                            }

                            Thread.Sleep(20);
                        }
                    }

                    Thread.Sleep(Common.AkStreamWebConfig.WaitEventTimeOutMSec);
                }
                catch
                {
                    //
                }
            }
        }
    }
}