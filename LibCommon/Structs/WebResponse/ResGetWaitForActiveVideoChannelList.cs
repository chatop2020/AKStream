using System;
using System.Collections.Generic;
using LibCommon.Structs.DBModels;
using LibCommon.Structs.WebRequest;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 未激活视频通道的返回结构
    /// </summary>
    [Serializable]
    public class ResGetWaitForActiveVideoChannelList
    {
        private ReqPaginationBase? _request;
        private long? _total;
        private List<VideoChannel>? _videoChannelList;

        public List<VideoChannel>? VideoChannelList
        {
            get => _videoChannelList;
            set => _videoChannelList = value;
        }

        public ReqPaginationBase? Request
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