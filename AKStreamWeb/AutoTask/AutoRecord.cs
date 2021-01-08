using System;
using System.Collections.Generic;
using System.Timers;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebRequest;

namespace AKStreamWeb.AutoTask
{
    public class AutoRecord:IDisposable
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

           

        }

        
        public void Dispose()
        {
            if (_loopTimer != null)
            {
                _loopTimer.Dispose();
                _loopTimer = null!;
            }
        }

        ~AutoRecord()
        {
            Dispose(); //释放非托管资源
        }

        public AutoRecord()
        {
            startTimer();
        }
    }
}