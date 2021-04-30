using System;
using System.Collections.Generic;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs
{
    [Serializable]
    public class MediaServerStreamPlayerInfo
    {
        private string _ipAddress = null!;
        private string? _params;
        private string _playerId;
        private ushort _port;
        private DateTime _startTime;


        /// <summary>
        /// 播放器的tcp id
        /// </summary>
        public string PlayerId
        {
            get => _playerId;
            set => _playerId = value;
        }

        /// <summary>
        /// 播放器的ip地址
        /// </summary>
        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// 播放器的端口
        /// </summary>
        public ushort Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 播放器参数
        /// </summary>
        public string? Params
        {
            get => _params;
            set => _params = value;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }
    }

    /// <summary>
    /// 流媒体服务中的流信息
    /// </summary>
    [Serializable]
    public class MediaServerStreamInfo
    {
        private string _app = null!;
        private bool? _isRecorded = false;
        private string _mediaServerId = null!;
        private string _mediaServerIp = null!;
        private string? _noGB28181Key = "";
        private string? _params = null;
        private List<MediaServerStreamPlayerInfo> _playerList = new List<MediaServerStreamPlayerInfo>();
        private List<string> _playUrl = new List<string>();
        private PushStreamSocketType? _pushSocketType = null;
        private ushort? _rptPort = 0;
        private DateTime _startTime;
        private string _stream = null!;
        private string _streamIp = null;
        private ushort _streamPort = 0;
        private string? _streamTcpId = null;
        private string _vhost = null!;
        private uint? ssrc;


        /// <summary>
        /// 流媒体服务的ID
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 流媒体服务的IP
        /// </summary>
        public string MediaServerIp
        {
            get => _mediaServerIp;
            set => _mediaServerIp = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 推流方的ip地址
        /// </summary>
        public string StreamIp
        {
            get => _streamIp;
            set => _streamIp = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// tcp推流时的tcpid,udp推流方式下为空
        /// </summary>
        public string? StreamTcpId
        {
            get => _streamTcpId;
            set => _streamTcpId = value;
        }

        /// <summary>
        /// 推流方的参数
        /// </summary>
        public string? Params
        {
            get => _params;
            set => _params = value;
        }


        /// <summary>
        /// 流媒体服务器的收流端口
        /// </summary>
        public ushort? RptPort
        {
            get => _rptPort;
            set => _rptPort = value;
        }

        /// <summary>
        /// 设备的推流的端口
        /// </summary>
        public ushort StreamPort
        {
            get => _streamPort;
            set => _streamPort = value;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }


        /// <summary>
        /// Vhost
        /// </summary>
        public string Vhost
        {
            get => _vhost;
            set => _vhost = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// APP
        /// </summary>
        public string App
        {
            get => _app;
            set => _app = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// 流ID
        /// </summary>
        public string Stream
        {
            get => _stream;
            set => _stream = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 播放地址
        /// </summary>
        public List<string> PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// ssrc
        /// </summary>
        public uint? Ssrc
        {
            get => ssrc;
            set => ssrc = value;
        }

        /// <summary>
        /// 是否正在录制
        /// </summary>
        public bool? IsRecorded
        {
            get => _isRecorded;
            set => _isRecorded = value;
        }

        /// <summary>
        /// 用于保存非28181流拉流时返回的key
        /// </summary>
        public string? NoGb28181Key
        {
            get => _noGB28181Key;
            set => _noGB28181Key = value;
        }

        /// <summary>
        /// 推流的socket类型 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PushStreamSocketType? PushSocketType
        {
            get => _pushSocketType;
            set => _pushSocketType = value;
        }

        /// <summary>
        /// 播放者列表
        /// </summary>
        public List<MediaServerStreamPlayerInfo> PlayerList
        {
            get => _playerList;
            set => _playerList = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}