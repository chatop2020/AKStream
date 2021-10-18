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
        private string _tag;
        private string _mediaServerId;
        private ushort _localRtpPort;
        private string _app;
        private bool _is_udp;
        private string _stream;
        private string _vhost;
        private string _localStream;

        public string RemoteIpAddress
        {
            get => _remoteIpAddress;
            set => _remoteIpAddress = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ushort RemotePort
        {
            get => _remotePort;
            set => _remotePort = value;
        }

        public string Ssrc
        {
            get => _ssrc;
            set => _ssrc = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string ChannelId
        {
            get => _channelId;
            set => _channelId = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string CallId
        {
            get => _callId;
            set => _callId = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int Cseq
        {
            get => _cseq;
            set => _cseq = value;
        }

        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public ushort LocalRtpPort
        {
            get => _localRtpPort;
            set => _localRtpPort = value;
        }

        public string App
        {
            get => _app;
            set => _app = value;
        }

      

        public bool Is_Udp
        {
            get => _is_udp;
            set => _is_udp = value;
        }

        public string Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }
        

        public string LocalStream
        {
            get => _localStream;
            set => _localStream = value;
        }
    }
}