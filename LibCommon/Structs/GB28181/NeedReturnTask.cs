using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;
using LibCommon.Structs.GB28181.XML;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SIPSorcery.SIP;
using Timer = System.Timers.Timer;

namespace LibCommon.Structs.GB28181
{
    /// <summary>
    /// 需要回复信息的sip方法任务
    /// </summary>
    public class NeedReturnTask : IDisposable
    {
        private static ConcurrentDictionary<string, NeedReturnTask> _needResponseRequests = null;
        private AutoResetEvent _autoResetEvent;
        private AutoResetEvent? _autoResetEvent2; //锁二，用于特殊的数据同步，如获取sip录像列表
        private string _callId;
        private CommandType _commandType;
        private DateTime _createTime;
        private object? _obj; //额外的通用类
        private SipChannel _sipChannel;
        private SipDevice _sipDevice;
        private SIPRequest _sipRequest;
        private int _timeout;
        private Timer _timeoutCheckTimer;

        public NeedReturnTask(ConcurrentDictionary<string, NeedReturnTask> c)
        {
            _createTime = DateTime.Now;
            _timeoutCheckTimer = new Timer(1000);
            _timeoutCheckTimer.Enabled = true; //启动Elapsed事件触发
            _timeoutCheckTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
            _timeoutCheckTimer.AutoReset = true; //需要自动reset
            _timeoutCheckTimer.Start(); //启动计时器
            _needResponseRequests = c;
        }


        /// <summary>
        /// sip请求
        /// </summary>
        public SIPRequest SipRequest
        {
            get => _sipRequest;
            set => _sipRequest = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 此次请求的callid
        /// </summary>
        public string CallId
        {
            get => _callId;
            set => _callId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 自动事件
        /// </summary>
        public AutoResetEvent AutoResetEvent
        {
            get => _autoResetEvent;
            set => _autoResetEvent = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 锁二，用于特殊的数据同步，如获取sip录像列表
        /// </summary>
        public AutoResetEvent? AutoResetEvent2
        {
            get => _autoResetEvent2;
            set => _autoResetEvent2 = value;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

        /// <summary>
        /// 操作针对的sipchannnel
        /// </summary>
        public SipChannel SipChannel
        {
            get => _sipChannel;
            set => _sipChannel = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// 操作针对的sipdevice
        /// </summary>
        public SipDevice SipDevice
        {
            get => _sipDevice;
            set => _sipDevice = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 额外的通用类型
        /// </summary>
        public object? Obj
        {
            get => _obj;
            set => _obj = value;
        }

        /// <summary>
        /// 命令类型 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType CommandType
        {
            get => _commandType;
            set => _commandType = value;
        }

        /// <summary>
        /// 记时器
        /// </summary>
        public Timer TimeoutCheckTimer
        {
            get => _timeoutCheckTimer;
            set => _timeoutCheckTimer = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void Dispose()
        {
            if (_timeoutCheckTimer != null)
            {
                _timeoutCheckTimer.Dispose();
                _timeoutCheckTimer = null!;
            }
        }

        ~NeedReturnTask()
        {
            Dispose(); //释放非托管资源
        }


        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _createTime).TotalMilliseconds > _timeout + 1000 && CommandType != CommandType.Playback)
            {
                _needResponseRequests.TryRemove(_callId, out _);
                Dispose();
            }
        }
    }
}