using System;
using System.Collections.Generic;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 音视频通道列表返回结构
    /// </summary>
    [Serializable]
    public class ResGetVideoChannelList
    {
        private ReqGetVideoChannelList? _request;
        private long? _total;
        private List<VideoChannel>? _videoChannelList;

        /// <summary>
        /// 音视频通道实例列表
        /// </summary>
        public List<VideoChannel>? VideoChannelList
        {
            get => _videoChannelList;
            set => _videoChannelList = value;
        }

        /// <summary>
        /// 请求结构
        /// </summary>
        public ReqGetVideoChannelList? Request
        {
            get => _request;
            set => _request = value;
        }

        /// <summary>
        /// 总数
        /// </summary>
        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}