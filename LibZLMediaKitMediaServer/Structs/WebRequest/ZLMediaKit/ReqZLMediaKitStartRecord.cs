using System;
using System.Text.Json.Serialization;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitStartRecord : ReqZLMediaKitRequestBase
    {
        private int? _type;
        private string? _vhost;
        private string? _app;
        private string? _stream;
        private string? _customized_path;

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

        public string? Customized_Path
        {
            get => _customized_path;
            set => _customized_path = value;
        }
    }
}