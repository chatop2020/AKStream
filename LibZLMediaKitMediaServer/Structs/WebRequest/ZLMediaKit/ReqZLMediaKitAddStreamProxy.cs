using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitAddStreamProxy : ReqZLMediaKitRequestBase
    {
        private string _vhost;
        private string _app;
        private string _stream;
        private string _url;
        private bool? _enable_hls;
        private bool? _enable_mp4;
        private int _rtp_type;


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
        /// 拉流地址，例如rtmp://live.hkstv.hk.lxdns.com/live/hks2
        /// </summary>
        public string Url
        {
            get => _url;
            set => _url = value;
        }

        /// <summary>
        /// 是否录制hls
        /// </summary>
        public bool? Enable_Hls
        {
            get => _enable_hls;
            set => _enable_hls = value;
        }

        /// <summary>
        /// 是否录制mp4
        /// </summary>
        public bool? Enable_Mp4
        {
            get => _enable_mp4;
            set => _enable_mp4 = value;
        }

        /// <summary>
        /// rtsp拉流时，拉流方式，0：tcp，1：udp，2：组播
        /// </summary>
        public int Rtp_Type
        {
            get => _rtp_type;
            set => _rtp_type = value;
        }
    }
}