using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    [Serializable]
    public class ReqForWebHookOnStreamNoneReader
    {
        private string? _app;
        private string? _mediaServerId;
        private string? _schema;
        private string? _stream;
        private string? _vhost;

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }
    }
}