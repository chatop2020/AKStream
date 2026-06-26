using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 请求结束向上级发送Rtp流
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitStopSendRtp : ReqZLMediaKitRequestBase
    {
        private string _app;
        private string _ssrc;
        private string _stream;
        private string _vhost;

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

        /// <summary>
        /// 根据ssrc关停某路rtp推流，置空时关闭所有流
        /// </summary>
        public string Ssrc
        {
            get => _ssrc;
            set => _ssrc = value;
        }
    }
}