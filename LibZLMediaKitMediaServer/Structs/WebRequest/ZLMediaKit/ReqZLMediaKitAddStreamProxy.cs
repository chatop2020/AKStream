using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitAddStreamProxy : ReqZLMediaKitRequestBase
    {
        private string _app;
        private int? _enable_hls;
        private int? _enable_mp4;
        private int _rtp_type;
        private string _stream;
        private float? _timeout_sec;
        private string _url;
        private string _vhost;
        private int? _retry_count=-1;//拉流失败时的重试拉流次数，-1为无限重试


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
        public int? Enable_Hls
        {
            get => _enable_hls;
            set => _enable_hls = value;
        }

        /// <summary>
        /// 是否录制mp4
        /// </summary>
        public int? Enable_Mp4
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

        /// <summary>
        /// 超时时间
        /// </summary>
        public float? Timeout_Sec
        {
            get => _timeout_sec;
            set => _timeout_sec = value;
        }

        /// <summary>
        /// 拉流失败时的重试拉流次数，-1为无限重试
        /// </summary>
        public int? Retry_Count
        {
            get => _retry_count;
            set => _retry_count = value;
        }
    }
}