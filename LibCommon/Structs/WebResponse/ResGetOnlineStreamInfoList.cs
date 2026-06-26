using System;
using System.Collections.Generic;
using LibCommon.Structs.WebRequest;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 返回在线音视频通道数据的结构
    /// </summary>
    [Serializable]
    public class ResGetOnlineStreamInfoList
    {
        private ReqGetOnlineStreamInfoList? _request;
        private long? _total;
        private List<VideoChannelMediaInfo>? _videoChannelMediaInfo;

        public List<VideoChannelMediaInfo>? VideoChannelMediaInfo
        {
            get => _videoChannelMediaInfo;
            set => _videoChannelMediaInfo = value;
        }

        public ReqGetOnlineStreamInfoList? Request
        {
            get => _request;
            set => _request = value;
        }

        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}