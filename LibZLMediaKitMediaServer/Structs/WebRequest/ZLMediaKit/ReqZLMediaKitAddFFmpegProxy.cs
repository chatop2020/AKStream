using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitAddFFmpegProxy : ReqZLMediaKitRequestBase
    {
        private string _dst_url;
        private int? _enable_hls;
        private int? _enable_mp4;
        private string? _ffmpeg_cmd_key;
        private string _src_url;
        private int _timeout_ms;

        /// <summary>
        /// 源地址
        /// </summary>
        public string Src_Url
        {
            get => _src_url;
            set => _src_url = value;
        }

        /// <summary>
        /// 推到目标地址，本地推送使用127.0.0.1为ip地址
        /// 如：rtmp://127.0.0.1/live/stream_form_ffmpeg
        /// </summary>
        public string Dst_Url
        {
            get => _dst_url;
            set => _dst_url = value;
        }

        /// <summary>
        /// 拉流超时毫秒
        /// </summary>
        public int Timeout_Ms
        {
            get => _timeout_ms;
            set => _timeout_ms = value;
        }

        /// <summary>
        /// 是否开起hls录制
        /// </summary>
        public int? Enable_Hls
        {
            get => _enable_hls;
            set => _enable_hls = value;
        }

        /// <summary>
        /// 是否开起mp4录制
        /// </summary>
        public int? Enable_Mp4
        {
            get => _enable_mp4;
            set => _enable_mp4 = value;
        }

        /// <summary>
        /// ffmpeg模板名称
        /// </summary>
        public string? Ffmpeg_Cmd_Key
        {
            get => _ffmpeg_cmd_key;
            set => _ffmpeg_cmd_key = value;
        }
    }
}