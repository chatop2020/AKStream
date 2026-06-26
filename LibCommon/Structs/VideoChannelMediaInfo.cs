using System;
using LibCommon.Enums;
using LibCommon.Structs.DBModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs
{
    /// <summary>
    /// 音视频通道流媒体信息
    /// </summary>
    [Serializable]
    public class VideoChannelMediaInfo : VideoChannel
    {
        private MediaServerStreamInfo? _mediaServerStreamInfo;
        private StreamSourceType? _streamSourceType;
        private long? _lostCount = 0;
        private byte? _timesToDelete = 2;

        public MediaServerStreamInfo? MediaServerStreamInfo
        {
            get => _mediaServerStreamInfo;
            set => _mediaServerStreamInfo = value;
        }

        /// <summary>
        /// 丢了多少次
        /// </summary>

        [JsonIgnore]
        public long? LostCount
        {
            get => _lostCount;
            set => _lostCount = value;
        }

        /// <summary>
        /// 丢多少次以后删掉
        /// </summary>
        [JsonIgnore]
        public byte? TimesToDelete
        {
            get => _timesToDelete;
            set => _timesToDelete = value;
        }

        /// <summary>
        /// 流来源类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StreamSourceType? StreamSourceType
        {
            get => _streamSourceType;
            set => _streamSourceType = value;
        }
    }
}