using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 向上级推rtp流请求结构
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitStartSendRtp : ReqZLMediaKitRequestBase
    {
        private string _app;
        private string _dst_port;
        private string _dst_url;
        private string _is_udp;
        private ushort _src_port;
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
        /// ssrc
        /// </summary>
        public string Ssrc
        {
            get => _ssrc;
            set => _ssrc = value;
        }

        /// <summary>
        /// 目标ip或域名
        /// </summary>
        public string Dst_Url
        {
            get => _dst_url;
            set => _dst_url = value;
        }

        /// <summary>
        /// 目标端口
        /// </summary>
        public string Dst_Port
        {
            get => _dst_port;
            set => _dst_port = value;
        }

        /// <summary>
        /// 是否udp
        /// </summary>
        public string Is_Udp
        {
            get => _is_udp;
            set => _is_udp = value;
        }

        /// <summary>
        /// 使用的本机端口，为0或不传时默认为随机端口
        /// </summary>
        public ushort Src_Port
        {
            get => _src_port;
            set => _src_port = value;
        }

        public override string ToString()
        {
            return
                $"{nameof(Vhost)}: {Vhost}, {nameof(App)}: {App}, {nameof(Stream)}: {Stream}, {nameof(Ssrc)}: {Ssrc}, {nameof(Dst_Url)}: {Dst_Url}, {nameof(Dst_Port)}: {Dst_Port}, {nameof(Is_Udp)}: {Is_Udp}, {nameof(Src_Port)}: {Src_Port}";
        }
    }
}