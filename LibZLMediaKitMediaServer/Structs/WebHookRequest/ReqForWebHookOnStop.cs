using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    [Serializable]
    public class ReqForWebHookOnStop
    {
        private string? _app;
        private long? _duration;
        private string? _id;
        private string? _ip;
        private string? _mediaServerId;
        private string? _params;
        private bool? _player; //true是播放器，false是推流器
        private ushort? _port;
        private string? _schema;
        private string? _stream;
        private ulong? _totalBytes;
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

        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public string? Params
        {
            get => _params;
            set => _params = value;
        }

        public bool? Player
        {
            get => _player;
            set => _player = value;
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

        public ulong? TotalBytes
        {
            get => _totalBytes;
            set => _totalBytes = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public string? Id
        {
            get => _id;
            set => _id = value;
        }
    }
}