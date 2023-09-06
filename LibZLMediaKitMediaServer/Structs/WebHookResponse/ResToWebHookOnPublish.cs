using System;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse
{
    [Serializable]
    public class ResToWebHookOnPublish : ResZLMediaKitResponseBase
    {
        // private bool? _enableHls;
        // private bool? _enableMP4;
        private string? _msg;

        private bool? _enable_hls;
        private bool? _enable_hls_fmp4;
        private bool? _enable_mp4;
        private bool? _enable_rtsp;
        private bool? _enable_rtmp;
        private bool? _enable_ts;
        private bool? _enable_fmp4;
        private bool? _hls_demand;
        private bool? _rtsp_demand;
        private bool? _rtmp_demand;
        private bool? _ts_demand;
        private bool? _fmp4_demand;
        private bool? _enable_audio;
        private bool? _add_mute_audio;
        private string? _mp4_save_path;
        private int? _mp4_max_second;
        private bool? _mp4_as_player;
        private string? _hls_save_path;
        private int? _modify_stamp;
        private UInt32? _continue_push_ms;
        private bool? _auto_close;
        private string _stream_replace;

        /// <summary>
        /// 是否转换成hls-mpegts协议
        /// </summary>
        public bool? Enable_Hls
        {
            get => _enable_hls;
            set => _enable_hls = value;
        }

        /// <summary>
        /// 是否转换成hls-fmp4协议
        /// </summary>
        public bool? Enable_Hls_Fmp4
        {
            get => _enable_hls_fmp4;
            set => _enable_hls_fmp4 = value;
        }

        /// <summary>
        /// 是否允许录制mp4文件
        /// </summary>
        public bool? Enable_Mp4
        {
            get => _enable_mp4;
            set => _enable_mp4 = value;
        }

        /// <summary>
        /// 是否转换rtsp协议
        /// </summary>
        public bool? Enable_Rtsp
        {
            get => _enable_rtsp;
            set => _enable_rtsp = value;
        }

        /// <summary>
        /// 是否转换rtmp/flv协议
        /// </summary>
        public bool? Enable_Rtmp
        {
            get => _enable_rtmp;
            set => _enable_rtmp = value;
        }

        /// <summary>
        /// 是否转换http-ts/ws-ts协议
        /// </summary>
        public bool? Enable_Ts
        {
            get => _enable_ts;
            set => _enable_ts = value;
        }

        /// <summary>
        /// 是否转换http-fmp4/ws-fmp4协议
        /// </summary>
        public bool? Enable_Fmp4
        {
            get => _enable_fmp4;
            set => _enable_fmp4 = value;
        }

        /// <summary>
        /// 是否有人观看才生成
        /// </summary>
        public bool? Hls_Demand
        {
            get => _hls_demand;
            set => _hls_demand = value;
        }

        /// <summary>
        ///是否有人观看才生成
        /// </summary>
        public bool? Rtsp_Demand
        {
            get => _rtsp_demand;
            set => _rtsp_demand = value;
        }

        /// <summary>
        /// 是否有人观看才生成
        /// </summary>
        public bool? Rtmp_Demand
        {
            get => _rtmp_demand;
            set => _rtmp_demand = value;
        }

        /// <summary>
        /// 是否有人观看才生成
        /// </summary>
        public bool? Ts_Demand
        {
            get => _ts_demand;
            set => _ts_demand = value;
        }

        /// <summary>
        /// 是否有人观看才生成
        /// </summary>
        public bool? Fmp4_Demand
        {
            get => _fmp4_demand;
            set => _fmp4_demand = value;
        }

        /// <summary>
        /// 转协议时是否开启音频
        /// </summary>
        public bool? Enable_Audio
        {
            get => _enable_audio;
            set => _enable_audio = value;
        }

        /// <summary>
        /// 转协议时，无音频是否添加静音aac音频
        /// </summary>
        public bool? Add_Mute_Audio
        {
            get => _add_mute_audio;
            set => _add_mute_audio = value;
        }

        /// <summary>
        /// mp4录制文件保存地址，置空使用默认
        /// </summary>
        public string Mp4_Save_Path
        {
            get => _mp4_save_path;
            set => _mp4_save_path = value;
        }

        /// <summary>
        /// 录制切片大小，单位秒
        /// </summary>
        public int? Mp4_Max_Second
        {
            get => _mp4_max_second;
            set => _mp4_max_second = value;
        }

        /// <summary>
        /// mp4录制是否当作观看者参与播放人数统计
        /// </summary>
        public bool? Mp4_As_Player
        {
            get => _mp4_as_player;
            set => _mp4_as_player = value;
        }

        /// <summary>
        /// hls文件保存根目录，置空使用默认
        /// </summary>
        public string Hls_Save_Path
        {
            get => _hls_save_path;
            set => _hls_save_path = value;
        }

        /// <summary>
        /// 是否开启时间戳覆盖，0:绝对时间戳  1：系统时间戳  2：相对时间戳
        /// </summary>
        public int? Modify_Stamp
        {
            get => _modify_stamp;
            set => _modify_stamp = value;
        }

        /// <summary>
        /// 断连续推延迟时间，单位毫秒，置空使用配置文件默认值
        /// </summary>
        public uint? Continue_Push_Ms
        {
            get => _continue_push_ms;
            set => _continue_push_ms = value;
        }

        /// <summary>
        /// 无关观看是否自动关闭流，不会触 发无人观看hook
        /// </summary>
        public bool? Auto_Close
        {
            get => _auto_close;
            set => _auto_close = value;
        }

        /// <summary>
        /// 是否修改流id,通过此参数可以自定义流id,比如替换ssrc值
        /// </summary>
        public string Stream_Replace
        {
            get => _stream_replace;
            set => _stream_replace = value;
        }

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}