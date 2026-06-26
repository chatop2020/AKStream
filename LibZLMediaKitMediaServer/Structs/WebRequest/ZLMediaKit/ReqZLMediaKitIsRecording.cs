using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 是否录制的请求结构
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitIsRecording : ReqZLMediaKitRequestBase
    {
        private string _app;
        private string _stream;
        private int _type;
        private string _vhost;

        /// <summary>
        /// 类型0为hls，1为mp4
        /// </summary>
        public int Type
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// vhost
        /// </summary>
        public string Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// app
        /// </summary>
        public string App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// stream
        /// </summary>
        public string Stream
        {
            get => _stream;
            set => _stream = value;
        }
    }
}