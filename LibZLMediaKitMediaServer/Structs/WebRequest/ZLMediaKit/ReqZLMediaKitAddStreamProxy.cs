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
        private int? _retry_count = -1; //拉流失败时的重试拉流次数，-1为无限重试
        private int? _enable_hls_fmp4; //是否转成hls-fmp4协议
        private int? _enable_rtsp; //是否转rtsp协议
        private int? _enable_rtmp; //是否转rtmp协议
        private int? _enable_ts; //是否转http-ts/ws-ts协议
        private int? _enable_fmp4; //是否转http-fmp4/ws-fmp4协议
        private int? _hls_demand; //该协议是否有人观看才生成
        private int? _rtsp_demand;
        private int? _rtmp_demand;
        private int? _ts_demand;
        private int? _fmp4_demand;
        private int? _enable_audio;
        private int? _add_mute_audio; //转协议时，无音频是否添加静音acc音频
        private string? _mp4_save_path; //mp4录制文件保存的根目录，置空使用默认
        private int? _mp4_max_second; //mp4录制切片大小，单位为秒
        private int? _mp4_as_player; //mp4录制是否当作观看者参与播放人数计数
        private string? _hls_save_path; //hls文件保存目录，置空使用默认
        private int? modify_stamp; //该流是否开启时间戳覆盖(0:绝对时间戳/1:系统时间戳/2:相对时间戳)
        private int? _auto_close; //无人观看是否自动关闭流(不触发无人观看hook)

        /// <summary>
        /// 是否转成hls-fmp4协议
        /// </summary>
        public int? Enable_Hls_Fmp4
        {
            get => _enable_hls_fmp4;
            set => _enable_hls_fmp4 = value;
        }

        /// <summary>
        /// 是否转rtsp协议
        /// </summary>
        public int? Enable_Rtsp
        {
            get => _enable_rtsp;
            set => _enable_rtsp = value;
        }

        /// <summary>
        /// 是否转rtmp协议
        /// </summary>
        public int? Enable_Rtmp
        {
            get => _enable_rtmp;
            set => _enable_rtmp = value;
        }

        /// <summary>
        /// 是否转http-ts/ws-ts协议
        /// </summary>
        public int? Enable_Ts
        {
            get => _enable_ts;
            set => _enable_ts = value;
        }

        /// <summary>
        /// 是否转http-fmp4/ws-fmp4协议
        /// </summary>
        public int? Enable_Fmp4
        {
            get => _enable_fmp4;
            set => _enable_fmp4 = value;
        }

        /// <summary>
        /// 该协议是否有人观看才生成
        /// </summary>
        public int? Hls_Demand
        {
            get => _hls_demand;
            set => _hls_demand = value;
        }

        /// <summary>
        /// 该协议是否有人观看才生成
        /// </summary>
        public int? Rtsp_Demand
        {
            get => _rtsp_demand;
            set => _rtsp_demand = value;
        }

        /// <summary>
        /// 该协议是否有人观看才生成
        /// </summary>
        public int? Rtmp_Demand
        {
            get => _rtmp_demand;
            set => _rtmp_demand = value;
        }

        /// <summary>
        /// 该协议是否有人观看才生成
        /// </summary>
        public int? Ts_Demand
        {
            get => _ts_demand;
            set => _ts_demand = value;
        }

        /// <summary>
        /// 该协议是否有人观看才生成
        /// </summary>
        public int? Fmp4_Demand
        {
            get => _fmp4_demand;
            set => _fmp4_demand = value;
        }

        /// <summary>
        /// 允许音频
        /// </summary>
        public int? Enable_Audio
        {
            get => _enable_audio;
            set => _enable_audio = value;
        }

        /// <summary>
        /// 转协议时，无音频是否添加静音acc音频
        /// </summary>
        public int? Add_Mute_Audio
        {
            get => _add_mute_audio;
            set => _add_mute_audio = value;
        }

        /// <summary>
        /// mp4录制文件保存的根目录，置空使用默认
        /// </summary>
        public string Mp4_Save_Path
        {
            get => _mp4_save_path;
            set => _mp4_save_path = value;
        }

        /// <summary>
        /// mp4录制切片大小，单位为秒
        /// </summary>
        public int? Mp4_Max_Second
        {
            get => _mp4_max_second;
            set => _mp4_max_second = value;
        }

        /// <summary>
        /// mp4录制是否当作观看者参与播放人数计数
        /// </summary>
        public int? Mp4_As_Player
        {
            get => _mp4_as_player;
            set => _mp4_as_player = value;
        }

        /// <summary>
        /// hls文件保存目录，置空使用默认
        /// </summary>
        public string Hls_Save_Path
        {
            get => _hls_save_path;
            set => _hls_save_path = value;
        }

        /// <summary>
        /// 该流是否开启时间戳覆盖(0:绝对时间戳/1:系统时间戳/2:相对时间戳)
        /// </summary>
        public int? Modify_Stamp
        {
            get => modify_stamp;
            set => modify_stamp = value;
        }

        /// <summary>
        /// 无人观看是否自动关闭流(不触发无人观看hook)
        /// </summary>
        public int? Auto_Close
        {
            get => _auto_close;
            set => _auto_close = value;
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