using System;
using System.Text;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181
{
    /// <summary>
    /// sip客户端配置文件
    /// </summary>
    public class SipClientConfig
    {
        private string _localIpAddress;
        private ushort _localPort;
        private string _sipServerIpAddress;
        private ushort _sipServerPort;
        private string _sipServerDeviceId;
        private string _realm;
        private string? _sipUsername;
        private string? _sipPassword;
        private int _keepAliveInterval;
        private int _keepAliveLostNumber;
        private string _sipDeviceId;
        private ushort _expiry = 3600;
        private EncodingType _encodingType;
        private Encoding _encoding;
        private string _akstreamWebHttpUrl;

    
        /// <summary>
        /// 本机ip地址
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string LocalIpAddress
        {
            get => _localIpAddress;
            set => _localIpAddress = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 本地端口
        /// </summary>
        public ushort LocalPort
        {
            get => _localPort;
            set => _localPort = value;
        }

        /// <summary>
        /// 远端服务ip地址
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string SipServerIpAddress
        {
            get => _sipServerIpAddress;
            set => _sipServerIpAddress = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 远端服务端口
        /// </summary>
        public ushort SipServerPort
        {
            get => _sipServerPort;
            set => _sipServerPort = value;
        }
        
        /// <summary>
        /// sip服务器的sip设备id
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string SipServerDeviceId
        {
            get => _sipServerDeviceId;
            set => _sipServerDeviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 域
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string Realm
        {
            get => _realm;
            set => _realm = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? SipUsername
        {
            get => _sipUsername;
            set => _sipUsername = value;
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string? SipPassword
        {
            get => _sipPassword;
            set => _sipPassword = value;
        }

        /// <summary>
        /// 心跳间隔（秒）
        /// </summary>
        public int KeepAliveInterval
        {
            get => _keepAliveInterval;
            set => _keepAliveInterval = value;
        }

        /// <summary>
        /// 多少次失败心跳算离线
        /// </summary>
        public int KeepAliveLostNumber
        {
            get => _keepAliveLostNumber;
            set => _keepAliveLostNumber = value;
        }

        /// <summary>
        /// Sip设备ID
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string SipDeviceId
        {
            get => _sipDeviceId;
            set => _sipDeviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 注册有效期（秒），默认3600
        /// </summary>
        public ushort Expiry
        {
            get => _expiry;
            set => _expiry = value;
        }
        
        /// <summary>
        /// 字符集类型
        /// UTF8
        /// GBK
        /// GB2312
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EncodingType EncodingType
        {
            get => _encodingType;
            set => _encodingType = value;
        }

        /// <summary>
        /// 执行的字符集
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [JsonIgnore]
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string AkstreamWebHttpUrl
        {
            get => _akstreamWebHttpUrl;
            set => _akstreamWebHttpUrl = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}