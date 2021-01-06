using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest
{
    /// <summary>
    /// 请求结构-当有播放者时触发
    /// </summary>
    [Serializable]
    public class ReqForWebHookOnPlay
    {
        private string? _app;
        private string? _id;
        private string? _ip;
        private string? _mediaServerId;
        private string? _params;
        private ushort? _port;
        private string? _schema;
        private string? _stream;
        private string? _vhost;


        /// <summary>
        /// 流媒体APP标记
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 流媒体针对每个播放用户的sessionid
        /// </summary>
        public string? Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 播放者IP地址
        /// </summary>
        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
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
        /// 端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 段
        /// </summary>
        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        /// <summary>
        /// 流媒体的StreamId标记
        /// </summary>
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        /// <summary>
        /// 流媒体Vhost标记
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }
    }
}