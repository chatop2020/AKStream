using System;
using System.Text.Json.Serialization;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitStopRecord : ReqZLMediaKitRequestBase
    {
        private string? _app;
        private string? _stream;
        private int? _type;
        private string? _vhost;


        [JsonIgnore]
        public int? Type
        {
            get => _type;
            set => _type = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }
    }
}