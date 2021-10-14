using System;

namespace LibCommon.Structs
{
    [Serializable]
    public class ShareInviteChannel
    {
        private string _deviceid;
        private string _ssrc;
        private ushort _port;
        private string _ipaddress;
        private DateTime _pushDateTime;
        private string _callId;
        private int _cseq;

        public string Deviceid
        {
            get => _deviceid;
            set => _deviceid = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Ssrc
        {
            get => _ssrc;
            set => _ssrc = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ushort Port
        {
            get => _port;
            set => _port = value;
        }

        public string Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value ?? throw new ArgumentNullException(nameof(value));
        }

        public DateTime PushDateTime
        {
            get => _pushDateTime;
            set => _pushDateTime = value;
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