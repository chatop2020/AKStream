using System;
using LibCommon.Enums;

namespace LibCommon.Structs.WebRequest
{
    [Serializable]
    public class ReqModifyVideoChannel : ReqActiveVideoChannel
    {
        private bool? _enable;
        private string? _mediaServerId;
        private DeviceStreamType? _deviceStreamType;
        private MethodByGetStream? _methodByGetStream;
        private string? _videoSrcUrl;
        private string? _deviceId;
        private string? _channelId;


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enable
        {
            get => _enable;
            set => _enable = value;
        }

        /// <summary>
        /// 流媒体服务器id
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 设备流类型
        /// </summary>
        public DeviceStreamType? DeviceStreamType
        {
            get => _deviceStreamType;
            set => _deviceStreamType = value;
        }

        /// <summary>
        /// 取流方式
        /// </summary>
        public MethodByGetStream? MethodByGetStream
        {
            get => _methodByGetStream;
            set => _methodByGetStream = value;
        }

        /// <summary>
        /// 流源地址
        /// </summary>
        public string? VideoSrcUrl
        {
            get => _videoSrcUrl;
            set => _videoSrcUrl = value;
        }

        /// <summary>
        /// gb28181设备的id
        /// </summary>
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// gb28181设备的通道id
        /// </summary>
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }
    }
}