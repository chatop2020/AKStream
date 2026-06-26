using System;
using LibCommon.Enums;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// 查询在线音视频通道列表的结构
    /// </summary>
    [Serializable]
    public class ReqGetOnlineStreamInfoList : ReqPaginationBase
    {
        private string? _mainId;
        private string? _mediaServerId;
        private StreamSourceType? _streamSourceType;
        private string? _videoChannelIp;

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }


        public string? MainId
        {
            get => _mainId;
            set => _mainId = value;
        }

        public string? VideoChannelIp
        {
            get => _videoChannelIp;
            set => _videoChannelIp = value;
        }

        /// <summary>
        /// 流的来源类型，直播/回放
        /// </summary>
        public StreamSourceType? StreamSourceType
        {
            get => _streamSourceType;
            set => _streamSourceType = value;
        }
    }
}