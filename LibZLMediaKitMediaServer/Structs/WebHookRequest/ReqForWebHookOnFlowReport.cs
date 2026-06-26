using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    [Serializable]
    public class ReqForWebHookOnFlowReport
    {
        private string? _app;
        private int? _duration;
        private string? _id;
        private string? _ip;
        private string _mediaServerId;
        private string? _params;
        private bool? _player;
        private int? _port;
        private string? _schema;
        private string? _stream;
        private ulong? _totalBytes;
        private string? _vhost;

        /// <summary>
        /// App
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 在线时长
        /// </summary>
        public int? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// 参数
        /// </summary>
        public string? Params
        {
            get => _params;
            set => _params = value;
        }

        /// <summary>
        /// 是否为播放者，false的话，就是音视频流
        /// </summary>
        public bool? Player
        {
            get => _player;
            set => _player = value;
        }

        /// <summary>
        /// 协议
        /// </summary>
        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        /// <summary>
        /// stream
        /// </summary>
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// 总流量
        /// </summary>
        public ulong? TotalBytes
        {
            get => _totalBytes;
            set => _totalBytes = value;
        }

        /// <summary>
        /// vhost
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public int? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// tcp情况下的tcpid
        /// </summary>
        public string? Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 流媒体服务器id
        /// </summary>
        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }
    }
}