using System.Threading;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibLogger;

namespace AKStreamWeb.AutoTask
{
    public class AutoLive
    {
        private void KeepLive()
        {
            while (true)
            {
                try
                {
                    var dbRet = ORMHelper.Db.Select<VideoChannel>().Where(x => x.Enabled.Equals(true))
                        .Where(x => x.AutoVideo.Equals(true)).Where(x => x.NoPlayerBreak.Equals(false)).ToList();
                    if (dbRet != null && dbRet.Count > 0)
                    {
                        foreach (var obj in dbRet)
                        {
                            if (obj != null)
                            {
                                var listRet = Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(obj.MainId));
                                if (listRet == null)
                                {
                                    var streamLiveRet = MediaServerService.StreamLive(obj.MediaServerId, obj.MainId,
                                        out ResponseStruct rs);
                                    if (!rs.Code.Equals(ErrorNumber.None) || streamLiveRet == null)
                                    {
                                        Logger.Warn(
                                            $"[{Common.LoggerHead}]->自动推流失败->{obj.MediaServerId}->{obj.MainId}->{JsonHelper.ToJson(Common.WebPerformanceInfo)}");
                                    }
                                    else
                                    {
                                        Logger.Info(
                                            $"[{Common.LoggerHead}]->自动推流成功->{obj.MediaServerId}->{obj.MainId}->{JsonHelper.ToJson(streamLiveRet)}");
                                    }
                                }
                            }

                            Thread.Sleep(100);
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch
                {
                    //
                }
            }
        }

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
    }
}