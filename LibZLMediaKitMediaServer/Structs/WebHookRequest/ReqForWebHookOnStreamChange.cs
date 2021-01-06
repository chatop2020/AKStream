using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    [Serializable]
    public class ReqForWebHookOnStreamChange
    {
        private string? _mediaServerId;
        private string? _app;
        private string? _schema;
        private string? _stream;
        private string? _vhost;
        private bool? _regist;

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

        public bool? Regist
        {
            get => _regist;
            set => _regist = value;
        }
    }
}