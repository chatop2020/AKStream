using System;

namespace LibCommon.Structs
{
    /// <summary>
    /// 共享视频流请求信息
    /// </summary>
    public class ShareInviteInfo
    {
        private string _remoteIpAddress;
        private ushort _remotePort;
        private string _ssrc;
        private string _channelId;
        private string _callId;
        private int _cseq;
        private string _toTag;
        private string _fromTag;
        private string _mediaServerId;
        private ushort _localRtpPort;
        private string _app;
        private bool _is_udp;
        private string _stream;
        private string _vhost;
        private string _localStream;
        private DateTime? _pushDateTime;

        /// <summary>
        /// 远程流媒体服务器ip地址
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string RemoteIpAddress
        {
            get => _remoteIpAddress;
            set => _remoteIpAddress = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 远程流媒体服务器端口
        /// </summary>
        public ushort RemotePort
        {
            get => _remotePort;
            set => _remotePort = value;
        }

        /// <summary>
        /// 远程要求的ssrc
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string Ssrc
        {
            get => _ssrc;
            set => _ssrc = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 本地共享通道号
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string ChannelId
        {
            get => _channelId;
            set => _channelId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// invite请求时的callid
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string CallId
        {
            get => _callId;
            set => _callId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// invite请求时的cseq
        /// </summary>
        public int Cseq
        {
            get => _cseq;
            set => _cseq = value;
        }

        /// <summary>
        /// invite请求时的tag
        /// </summary>
        public string ToTag
        {
            get => _toTag;
            set => _toTag = value;
        }

        /// <summary>
        /// invite请求时的tag
        /// </summary>
        public string FromTag
        {
            get => _fromTag;
            set => _fromTag = value;
        }

        /// <summary>
        /// 共享流所在的流媒体服务器id
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 申请的rtp(发送)端口
        /// </summary>
        public ushort LocalRtpPort
        {
            get => _localRtpPort;
            set => _localRtpPort = value;
        }

        /// <summary>
        /// app
        /// </summary>
        public string App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 是否使用udp推流
        /// </summary>

        public bool Is_Udp
        {
            get => _is_udp;
            set => _is_udp = value;
        }

        /// <summary>
        /// 音视频通道的mainid
        /// </summary>
        public string Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// vhost
        /// </summary>
        public string Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }


        /// <summary>
        /// 本地生成的stream
        /// </summary>
        public string LocalStream
        {
            get => _localStream;
            set => _localStream = value;
        }

        /// <summary>
        /// 推流时间
        /// </summary>
        public DateTime? PushDateTime
        {
            get => _pushDateTime;
            set => _pushDateTime = value;
        }
    }
}