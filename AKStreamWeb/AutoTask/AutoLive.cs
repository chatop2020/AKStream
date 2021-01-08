using System;
using System.Threading;
using System.Timers;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs.DBModels;
using LibLogger;
using Timer = System.Timers.Timer;

namespace AKStreamWeb.AutoTask
{
    public class AutoLive: IDisposable
    {
        private Timer _loopTimer;
        
        private void startTimer()
        {
            if (_loopTimer == null)
            {
                _loopTimer = new Timer(1000);
                _loopTimer.Enabled = true; //启动Elapsed事件触发
                _loopTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
                _loopTimer.AutoReset = true; //需要自动reset
                _loopTimer.Start(); //启动计时器
            }
        }
        
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var dbRet = ORMHelper.Db.Select<VideoChannel>().Where(x => x.Enabled.Equals(true))
                .Where(x => x.AutoVideo.Equals(true)).Where(x => x.NoPlayerBreak.Equals(false)).ToList();
            if (dbRet != null && dbRet.Count > 0)
            {
                foreach (var obj in dbRet)
                {
                    if (obj != null)
                    {

                        var listRet=Common.VideoChannelMediaInfos.FindLast(x => x.MainId.Equals(obj.MainId));
                        if (listRet == null)
                        {
                            var streamLiveRet = MediaServerService.StreamLive(obj.MediaServerId, obj.MainId,
                                out ResponseStruct rs);
                            if (!rs.Code.Equals(ErrorNumber.None) || streamLiveRet == null)
                            {
                                Logger.Warn($"[{Common.LoggerHead}]->自动推流失败->{obj.MediaServerId}->{obj.MainId}->{JsonHelper.ToJson(Common.WebPerformanceInfo)}");

                            }
                            else
                            {
                                Logger.Info($"[{Common.LoggerHead}]->自动推流成功->{obj.MediaServerId}->{obj.MainId}->{JsonHelper.ToJson(streamLiveRet)}");
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }

        
        public void Dispose()
        {
            if (_loopTimer != null)
            {
                _loopTimer.Dispose();
                _loopTimer = null!;
            }
        }

        ~AutoLive()
        {
            Dispose(); //释放非托管资源
        }

        public AutoLive()
        {
            startTimer();  
        }
    }
}