using System;

namespace AKStreamWeb.Misc
{
    [Serializable]
    public class AKStreamWebConfig
    {
        private string _accessKey;
        private string _dbType;
        private int _httpClientTimeoutSec;
        private bool _mediaServerFirstToRestart = true;
        private string _ormConnStr;
        private int _waitEventTimeOutMSec = 10000;
        private int _waitSipRequestTimeOutMSec = 5000;
        private ushort _webApiPort = 5800;
        private ushort _deletedRecordsExpiredDays = 30;
        private bool _enableGB28181Client = false;
        private bool? _enableGB28181Server = false;
        private string? _ZlmFlvPrefix = "live";
        private string? _listenIP = "127.0.0.1";
        private bool _localizationKingBaseDB = false;
        private bool _forwardUnmanagedRtmpRtspRtcStream = true;
        private string? _forwardUrlIn = "";
        private string? _forwardUrlOut = "";
        private string? _forwardUrlOnRecord = "";


        /// <summary>
        /// 流媒体服务器首次注册是否要求其重新mediaserver
        /// </summary>
        public bool MediaServerFirstToRestart
        {
            get => _mediaServerFirstToRestart;
            set => _mediaServerFirstToRestart = value;
        }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType
        {
            get => _dbType;
            set => _dbType = value;
        }


        /// <summary>
        /// 数据库的连接串
        /// </summary>
        public string OrmConnStr
        {
            get => _ormConnStr;
            set => _ormConnStr = value;
        }

        /// <summary>
        /// WebApi端口
        /// </summary>
        public ushort WebApiPort
        {
            get => _webApiPort;
            set => _webApiPort = value;
        }

        /// <summary>
        /// 访问webapi的密钥
        /// </summary>
        public string AccessKey
        {
            get => _accessKey;
            set => _accessKey = value;
        }

        /// <summary>
        /// http客户端超时时间（秒）
        /// </summary>
        public int HttpClientTimeoutSec
        {
            get => _httpClientTimeoutSec;
            set => _httpClientTimeoutSec = value;
        }

        /// <summary>
        /// 等待事件发生的超时时间
        /// </summary>
        public int WaitEventTimeOutMSec
        {
            get => _waitEventTimeOutMSec;
            set => _waitEventTimeOutMSec = value;
        }

        /// <summary>
        /// Sip操作超时时间
        /// </summary>
        public int WaitSipRequestTimeOutMSec
        {
            get => _waitSipRequestTimeOutMSec;
            set => _waitSipRequestTimeOutMSec = value;
        }

        /// <summary>
        /// 录像记录中被删除的记录，保留多久天后彻底删除
        /// </summary>
        public ushort DeletedRecordsExpiredDays
        {
            get => _deletedRecordsExpiredDays;
            set => _deletedRecordsExpiredDays = value;
        }

        /// <summary>
        /// 是否启用gb28181客户端
        /// </summary>
        public bool EnableGB28181Client
        {
            get => _enableGB28181Client;
            set => _enableGB28181Client = value;
        }

        /// <summary>
        /// 是否启用gb28181服务
        /// </summary>
        public bool? EnableGB28181Server
        {
            get => _enableGB28181Server;
            set => _enableGB28181Server = value;
        }

        /// <summary>
        /// 新的Zlm播放flv时，url规格变了，要加一个live，由于老版本zlm不需要，所以这里多加一个配置项进行控制
        /// </summary>
        public string? ZlmFlvPrefix
        {
            get => _ZlmFlvPrefix;
            set => _ZlmFlvPrefix = value;
        }

        /// <summary>
        /// 监听ip地址
        /// </summary>
        public string ListenIp
        {
            get => _listenIP;
            set => _listenIP = value;
        }


        /// <summary>
        /// 是否使用国产化的Kingbase数据库（人大金仓）
        /// </summary>
        public bool LocalizationKingBaseDb
        {
            get => _localizationKingBaseDB;
            set => _localizationKingBaseDB = value;
        }

        /// <summary>
        /// 是否向外部转发未管理的rtmp流(直播支持)
        /// </summary>
        public bool ForwardUnmanagedRtmpRtspRtcStream
        {
            get => _forwardUnmanagedRtmpRtspRtcStream;
            set => _forwardUnmanagedRtmpRtspRtcStream = value;
        }

        /// <summary>
        /// 流接入的转发地址
        /// </summary>
        public string ForwardUrlIn
        {
            get => _forwardUrlIn;
            set => _forwardUrlIn = value;
        }
        /// <summary>
        /// 流注销的转发地址
        /// </summary>
        public string ForwardUrlOut
        {
            get => _forwardUrlOut;
            set => _forwardUrlOut = value;
        }

        /// <summary>
        /// 有录制消息的时候的转发
        /// </summary>
        public string ForwardUrlOnRecord
        {
            get => _forwardUrlOnRecord;
            set => _forwardUrlOnRecord = value;
        }
    }
}