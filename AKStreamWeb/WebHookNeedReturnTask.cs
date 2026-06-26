using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AKStreamWeb
{
    public class WebHookNeedReturnTask : IDisposable
    {
        private static ConcurrentDictionary<string, WebHookNeedReturnTask> _webHookNeedReturnTask = null;
        private AutoResetEvent _autoResetEvent;
        private DateTime _createTime;
        private string _tag;
        private int _timeout;
        private Timer _timeoutCheckTimer;
        private object? otherObj = null;

        public WebHookNeedReturnTask(ConcurrentDictionary<string, WebHookNeedReturnTask> c)
        {
            _createTime = DateTime.Now;
            _timeoutCheckTimer = new Timer(1000);
            _timeoutCheckTimer.Enabled = true; //启动Elapsed事件触发
            _timeoutCheckTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
            _timeoutCheckTimer.AutoReset = true; //需要自动reset
            _timeoutCheckTimer.Start(); //启动计时器
            _webHookNeedReturnTask = c;
        }

        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public AutoResetEvent AutoResetEvent
        {
            get => _autoResetEvent;
            set => _autoResetEvent = value;
        }

        public int Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

        public Timer TimeoutCheckTimer
        {
            get => _timeoutCheckTimer;
            set => _timeoutCheckTimer = value;
        }

        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        public object? OtherObj
        {
            get => otherObj;
            set => otherObj = value;
        }

        public void Dispose()
        {
            if (_timeoutCheckTimer != null)
            {
                _timeoutCheckTimer.Dispose();
                _timeoutCheckTimer = null!;
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _createTime).Milliseconds > _timeout + 1000)
            {
                _webHookNeedReturnTask.TryRemove(_tag, out _);
                Dispose();
            }
        }
    }
}