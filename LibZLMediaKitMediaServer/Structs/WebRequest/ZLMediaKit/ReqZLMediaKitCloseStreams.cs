using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitCloseStreams : ReqZLMediaKitRequestBase
    {
        private string? _schema;
        private string? _vhost;
        private string? _app;
        private string? _stream;
        private bool? _force;


        public string? Schema
        {
            get => _schema;
            set => _schema = value;
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

        //此参数如果为true时，将会导致被结束的流不再被cameraKeeper重复推送
        public bool? Force
        {
            get => _force;
            set => _force = value;
        }
    }
}