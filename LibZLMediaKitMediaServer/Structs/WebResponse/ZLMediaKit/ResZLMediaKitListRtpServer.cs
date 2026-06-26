using System;
using System.Collections.Generic;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class RtpPortData
    {
        private ushort? _port;

        private string? _stream_id;

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public string? Stream_Id
        {
            get => _stream_id;
            set => _stream_id = value;
        }
    }

    [Serializable]
    public class ResZLMediaKitListRtpServer : ResZLMediaKitResponseBase
    {
        private List<RtpPortData> _data;

        public List<RtpPortData> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}