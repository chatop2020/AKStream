using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using LibCommon.Structs.GB28181.XML;
using Newtonsoft.Json;
using SIPSorcery.SIP;

namespace LibCommon.Structs.GB28181
{
    [Serializable]
    public class SipDevice : IDisposable
    {
        private IPAddress? _ipAddress;
        private int _port;
        private string _deviceId = null!;
        private SIPEndPoint? _remoteEndPoint;
        private SIPEndPoint? _localSipEndPoint;
        private List<SipChannel> _sipChannels = new List<SipChannel>();
        private DeviceInfo _deviceInfo = new DeviceInfo();
        private DateTime _registerTime;
        private string? _username;
        private string? _password;
        private DateTime _keepAliveTime;
        private int _keepAliveLostTime;
        private Timer _keepAliveCheckTimer;
        private SIPRequest? _lastSipRequest;
        private SIPResponse? _lastSipResponse;
        private SIPURI? _contactUri;
        private SIPChannel? _sipChannelLayout;
        private SipServerConfig _sipServerConfig;
        private DeviceStatus _deviceStatus = null;
        private bool _isReday = false;

        public event GCommon.DoKickSipDevice KickMe = null!;


        /// <summary>
        /// 对sip通道操作时的锁
        /// </summary>
        [JsonIgnore] 
        [LiteDB.BsonIgnore]
        public object SipChannelOptLock = new object();

        public void Dispose()
        {
            if (_keepAliveCheckTimer != null)
            {
                _keepAliveCheckTimer.Dispose();
                _keepAliveCheckTimer = null!;
            }
        }

        ~SipDevice()
        {
            Dispose(); //释放非托管资源
        }

        [LiteDB.BsonIgnore]
        /// <summary>
        /// 设备ip地址
        /// </summary>
        public IPAddress? IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 设备端口
        /// </summary>
        public int Port
        {
            get => _port;
            set => _port = value;
        }


        /// <summary>
        /// sip设备ip端口协议
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPEndPoint? RemoteEndPoint
        {
            get => _remoteEndPoint;
            set => _remoteEndPoint = value;
        }

        /// <summary>
        /// sip服务ip端口协议
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPEndPoint? LocalSipEndPoint
        {
            get => _localSipEndPoint;
            set => _localSipEndPoint = value;
        }


        /// <summary>
        /// 设备所属通道信息
        /// </summary>
        public List<SipChannel> SipChannels
        {
            get => _sipChannels;
            set => _sipChannels = value;
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfo DeviceInfo
        {
            get => _deviceInfo;
            set => _deviceInfo = value;
        }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime
        {
            get => _registerTime;
            set => _registerTime = value;
        }

        /// <summary>
        /// 注册用户名
        /// </summary>
        public string? Username
        {
            get => _username;
            set => _username = value;
        }

        /// <summary>
        /// 注册密码
        /// </summary>
        public string? Password
        {
            get => _password;
            set => _password = value;
        }

        /// <summary>
        /// 最后一次心跳时间
        /// </summary>
        public DateTime KeepAliveTime
        {
            get => _keepAliveTime;
            set => _keepAliveTime = value;
        }

        /// <summary>
        /// 心跳已连续丢失多少次
        /// </summary>
        public int KeepAliveLostTime
        {
            get => _keepAliveLostTime;
            set => _keepAliveLostTime = value;
        }


        private void startTimer()
        {
            if (_keepAliveCheckTimer == null)
            {
                _keepAliveCheckTimer = new Timer(_sipServerConfig.KeepAliveInterval * 1000);
                _keepAliveCheckTimer.Enabled = true; //启动Elapsed事件触发
                _keepAliveCheckTimer.Elapsed += OnTimedEvent; //添加触发事件的函数
                _keepAliveCheckTimer.AutoReset = true; //需要自动reset
                _keepAliveCheckTimer.Start(); //启动计时器
            }
        }

        public SipDevice()
        {
        }

        /// <summary>
        ///最后一次SipRequest
        /// </summary>

        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPRequest? LastSipRequest
        {
            get => _lastSipRequest;
            set => _lastSipRequest = value;
        }

        /// <summary>
        /// 最后一次sipresponse
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPResponse? LastSipResponse
        {
            get => _lastSipResponse;
            set => _lastSipResponse = value;
        }


        /// <summary>
        /// 注册时候的uri
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPURI? ContactUri
        {
            get => _contactUri;
            set => _contactUri = value;
        }


        /// <summary>
        /// 设备所在的Sip通道实例
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SIPChannel? SipChannelLayout
        {
            get => _sipChannelLayout;
            set => _sipChannelLayout = value;
        }

        /// <summary>
        /// sip服务器的配置类
        /// </summary>
        [JsonIgnore]
        [LiteDB.BsonIgnore]
        public SipServerConfig SipServerConfig
        {
            get => _sipServerConfig;
            set => _sipServerConfig = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 设备的状态信息
        /// </summary>
        public DeviceStatus DeviceStatus
        {
            get => _deviceStatus;
            set => _deviceStatus = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 设备是否已经准备好，第一次心跳以后算准备就绪
        /// </summary>
        public bool IsReday
        {
            get => _isReday;
            set => _isReday = value;
        }

        public SipDevice(SipServerConfig sipServerConfig)
        {
            _sipServerConfig = sipServerConfig;
            startTimer();
        }


        /// <summary>
        /// 定时触发，检查心跳周期并计算失去多少次心跳
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _keepAliveTime).TotalSeconds > _sipServerConfig.KeepAliveInterval + 1)
            {
                _keepAliveLostTime++;
            }
            else
            {
                _keepAliveLostTime--;
                if (_keepAliveLostTime < 0)
                {
                    _keepAliveLostTime = 0;
                }
            }

            //做踢除
            if (_keepAliveLostTime >= _sipServerConfig.KeepAliveLostNumber)
            {
                if (KickMe != null)
                {
                    _keepAliveCheckTimer.Enabled = false;
                    _keepAliveCheckTimer.Stop();
                    KickMe?.Invoke(this!);
                }
            }
        }
    }
}