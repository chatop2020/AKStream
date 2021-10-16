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
    }
}