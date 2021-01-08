using System;
using LibCommon.Enums;

namespace LibCommon.Structs.WebRequest
{
    [Serializable]
    public class ReqPtzCtrl
    {
        private PTZCommandType _ptzCommandType;
        private int _speed;
        private string _deviceId;
        private string? _channelId;

        /// <summary>
        /// 控制类型
        /// </summary>
        public PTZCommandType PtzCommandType
        {
            get => _ptzCommandType;
            set => _ptzCommandType = value;
        }

        /// <summary>
        /// 移动速度0-255
        /// </summary>
        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        /// <summary>
        /// 设备id
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 通道id
        /// </summary>
        public string? ChannelId
        {
            get => _channelId;
            set => _channelId = value;
        }
    }
}