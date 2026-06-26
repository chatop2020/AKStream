using System;
using System.Text.Json.Serialization;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitStartRecord : ReqZLMediaKitRequestBase
    {
        private string? _app;
        private string? _customized_path;
        private int? _max_second = 0;
        private string? _stream;
        private int? _type;
        private string? _vhost;

        [JsonIgnore]
        public int? Type
        {
            get => _type;
            set => _type = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? Customized_Path
        {
            get => _customized_path;
            set => _customized_path = value;
        }

        /// <summary>
        /// 指定通道录制文件录制时长（秒）
        /// </summary>
        public int? Max_Second
        {
            get => _max_second;
            set => _max_second = value;
        }
    }
}