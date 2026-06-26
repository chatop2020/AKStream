using System;
using System.Text.Json.Serialization;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitOpenRtpPort : ReqZLMediaKitRequestBase
    {
        private int? _enable_tcp;
        private ushort? _port;
        private string? _stream_id;

        [JsonIgnore]
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        [JsonIgnore]
        public int? Enable_Tcp
        {
            get => _enable_tcp;
            set => _enable_tcp = value;
        }

        public string? Stream_Id
        {
            get => _stream_id;
            set => _stream_id = value;
        }
    }
}