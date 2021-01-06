using System;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs
{
    [Serializable]
    public class PushMediaInfo
    {
        private string _mediaServerIpAddress;
        private PushStreamSocketType _pushStreamSocketType;
        private ushort _streamPort;


        /// <summary>
        /// 流媒体服务器ip地址
        /// </summary>
        public string MediaServerIpAddress
        {
            get => _mediaServerIpAddress;
            set => _mediaServerIpAddress = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// 推流socket类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PushStreamSocketType PushStreamSocketType
        {
            get => _pushStreamSocketType;
            set => _pushStreamSocketType = value;
        }

        /// <summary>
        /// 流媒体服务器收流端口
        /// </summary>
        public ushort StreamPort
        {
            get => _streamPort;
            set => _streamPort = value;
        }
    }
}