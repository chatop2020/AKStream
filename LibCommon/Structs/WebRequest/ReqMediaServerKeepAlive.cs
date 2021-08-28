using System;
using System.Collections.Generic;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// 流媒体服务器的心跳结构
    /// </summary>
    [Serializable]
    public class ReqMediaServerKeepAlive
    {
        private string _accessKey;
        private bool _firstPost = false;
        private string _ipV4Address;
        private string _ipV6Address;
        private ushort _keeperWebApiPort;
        private string _mediaServerId;
        private bool _mediaServerIsRunning = false;
        private int _mediaServerPid;
        private PerformanceInfo? _performanceInfo;
        private bool _randomPort;
        private List<KeyValuePair<double, string>> _recordPathList;
        private ushort _rtpPortMax; //仅使用min-max中的偶数类端口
        private ushort _rtpPortMin;
        private string _secret;
        private DateTime _serverDateTime; //流媒体服务器当前时间
        private bool _useSsl;
        private string _version;
        private ushort _zlmHttpPort;
        private ushort _zlmHttpsPort;
        private uint _zlmRecordFileSec;
        private ushort _zlmRtmpPort;
        private ushort _zlmRtmpsPort;
        private ushort _zlmRtspPort;
        private ushort _zlmRtspsPort;
     


        /// <summary>
        /// ipv4地址
        /// </summary>
        public string IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// ipv6地址
        /// </summary>
        public string IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// keeper的WebApi端口
        /// </summary>
        public ushort KeeperWebApiPort
        {
            get => _keeperWebApiPort;
            set => _keeperWebApiPort = value;
        }

        /// <summary>
        /// ZLM的Secret值
        /// </summary>
        public string Secret
        {
            get => _secret;
            set => _secret = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// zlm的id
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// zlm的pid
        /// </summary>
        public int MediaServerPid
        {
            get => _mediaServerPid;
            set => _mediaServerPid = value;
        }

        /// <summary>
        /// 自定义视频存储路径
        /// </summary>
        public List<KeyValuePair<double, string>> RecordPathList
        {
            get => _recordPathList;
            set => _recordPathList = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// rtp开放范围(最小)
        /// </summary>
        public ushort RtpPortMin
        {
            get => _rtpPortMin;
            set => _rtpPortMin = value;
        }


        /// <summary>
        /// rtp开放范围（最大）
        /// </summary>
        public ushort RtpPortMax
        {
            get => _rtpPortMax;
            set => _rtpPortMax = value;
        }

        /// <summary>
        /// 是否zlm自动生成rtp端口
        /// </summary>
        public bool RandomPort
        {
            get => _randomPort;
            set => _randomPort = value;
        }

        /// <summary>
        /// 服务器当前时间
        /// </summary>
        public DateTime ServerDateTime
        {
            get => _serverDateTime;
            set => _serverDateTime = value;
        }

        /// <summary>
        /// 流媒体服务器的性能情况
        /// </summary>
        public PerformanceInfo? PerformanceInfo
        {
            get => _performanceInfo;
            set => _performanceInfo = value;
        }

        public ushort ZlmHttpPort
        {
            get => _zlmHttpPort;
            set => _zlmHttpPort = value;
        }

        public ushort ZlmHttpsPort
        {
            get => _zlmHttpsPort;
            set => _zlmHttpsPort = value;
        }

        public ushort ZlmRtspPort
        {
            get => _zlmRtspPort;
            set => _zlmRtspPort = value;
        }

        public ushort ZlmRtmpPort
        {
            get => _zlmRtmpPort;
            set => _zlmRtmpPort = value;
        }

        public ushort ZlmRtspsPort
        {
            get => _zlmRtspsPort;
            set => _zlmRtspsPort = value;
        }

        public ushort ZlmRtmpsPort
        {
            get => _zlmRtmpsPort;
            set => _zlmRtmpsPort = value;
        }

        public uint ZlmRecordFileSec
        {
            get => _zlmRecordFileSec;
            set => _zlmRecordFileSec = value;
        }

        public bool UseSsl
        {
            get => _useSsl;
            set => _useSsl = value;
        }

        /// <summary>
        /// 第一次上报，要将控制服务器上的实例删掉
        /// </summary>
        public bool FirstPost
        {
            get => _firstPost;
            set => _firstPost = value;
        }

        public string AccessKey
        {
            get => _accessKey;
            set => _accessKey = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 流媒体服务器是否正在运行
        /// </summary>
        public bool MediaServerIsRunning
        {
            get => _mediaServerIsRunning;
            set => _mediaServerIsRunning = value;
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get => _version;
            set => _version = value ?? throw new ArgumentNullException(nameof(value));
        }

       
    }
}