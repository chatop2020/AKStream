using System;
using System.Collections.Generic;

namespace AKStreamKeeper.Misc
{
    /// <summary>
    /// AKStreamKeeper配置文件结构
    /// </summary>
    [Serializable]
    public class AKStreamKeeperConfig
    {
        private string _ipV4Address;
        private string _ipV6Address;
        private ushort _webApiPort;
        private string _mediaServerPath;
        private string _akStreamWebRegisterUrl;
        private List<string> _customRecordPathList;
        private bool _useSSL = false;
        private ushort _minRtpPort = 10001;
        private ushort _maxRtpPort = 30000;
        private bool _randomPort = false;
        private int? _recordSec = 120;
        private string _ffmpegPath;
        private string _accessKey;
        private int _rtpPortCDTime;
        private int _httpClientTimeoutSec;


        /// <summary>
        /// ipv4地址
        /// </summary>
        public string IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value;
        }

        /// <summary>
        /// ipv6地址
        /// </summary>
        public string IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value;
        }

        /// <summary>
        /// webapi端口
        /// </summary>
        public ushort WebApiPort
        {
            get => _webApiPort;
            set => _webApiPort = value;
        }

        /// <summary>
        /// 流媒体服务器路径
        /// </summary>
        public string MediaServerPath
        {
            get => _mediaServerPath;
            set => _mediaServerPath = value;
        }

        /// <summary>
        /// 流媒体注册地址
        /// </summary>
        public string AkStreamWebRegisterUrl
        {
            get => _akStreamWebRegisterUrl;
            set => _akStreamWebRegisterUrl = value;
        }

        /// <summary>
        /// 自定义录像路径列表
        /// </summary>
        public List<string> CustomRecordPathList
        {
            get => _customRecordPathList;
            set => _customRecordPathList = value;
        }

        /// <summary>
        /// 流媒体服务器是否使用SSL安全连接
        /// </summary>
        public bool UseSsl
        {
            get => _useSSL;
            set => _useSSL = value;
        }

        /// <summary>
        /// rtp端口范围（小）
        /// </summary>
        public ushort MinRtpPort
        {
            get => _minRtpPort;
            set => _minRtpPort = value;
        }

        /// <summary>
        /// rtp端口范围（大）
        /// </summary>
        public ushort MaxRtpPort
        {
            get => _maxRtpPort;
            set => _maxRtpPort = value;
        }

        /// <summary>
        /// rtp端口是否由zlm随机生成
        /// </summary>
        public bool RandomPort
        {
            get => _randomPort;
            set => _randomPort = value;
        }

        /// <summary>
        /// 录制文件时长(秒)
        /// </summary>
        public int? RecordSec
        {
            get => _recordSec;
            set => _recordSec = value;
        }

        /// <summary>
        /// ffmpeg的可执行文件路径
        /// </summary>
        public string FFmpegPath
        {
            get => _ffmpegPath;
            set => _ffmpegPath = value;
        }

        /// <summary>
        /// 访问webapi需要携带的key
        /// </summary>
        public string AccessKey
        {
            get => _accessKey;
            set => _accessKey = value;
        }

        /// <summary>
        /// rtp端口冷却时间默认3600秒，以保证端口被重复使用的可能性降低，以解决推流异常情况
        /// </summary>
        public int RtpPortCdTime
        {
            get => _rtpPortCDTime;
            set => _rtpPortCDTime = value;
        }

        /// <summary>
        /// httpclient超时时间（秒）
        /// </summary>
        public int HttpClientTimeoutSec
        {
            get => _httpClientTimeoutSec;
            set => _httpClientTimeoutSec = value;
        }
    }
}