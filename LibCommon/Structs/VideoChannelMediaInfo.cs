using System;
using LibCommon.Structs.DBModels;

namespace LibCommon.Structs
{
    /// <summary>
    /// 音视频通道流媒体信息
    /// </summary>
    [Serializable]
    public class VideoChannelMediaInfo : VideoChannel
    {
       
        private MediaServerStreamInfo? _mediaServerStreamInfo;
        
        public MediaServerStreamInfo? MediaServerStreamInfo
        {
            get => _mediaServerStreamInfo;
            set => _mediaServerStreamInfo = value;
        }
    }
}