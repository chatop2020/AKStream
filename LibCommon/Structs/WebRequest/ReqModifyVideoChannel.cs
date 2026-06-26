using System;
using LibCommon.Enums;

namespace LibCommon.Structs.WebRequest
{
    [Serializable]
    public class ReqModifyVideoChannel : ReqActiveVideoChannel
    {
        private string? _channelId;
        private string? _deviceId;
        private DeviceStreamType? _deviceStreamType;
        private bool? _enabled;
        private string? _mediaServerId;
        private MethodByGetStream? _methodByGetStream;
        private string? _videoSrcUrl;
        private bool? _isShareChannel;
        private string? _shareUrl;
        private string? _shareDeviceId;


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled
        {
            get => _enabled;
            set => _enabled = value;
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

        public bool? IsShareChannel
        {
            get => _isShareChannel;
            set => _isShareChannel = value;
        }

        public string? ShareUrl
        {
            get => _shareUrl;
            set => _shareUrl = value;
        }

        public string? ShareDeviceId
        {
            get => _shareDeviceId;
            set => _shareDeviceId = value;
        }
    }
}