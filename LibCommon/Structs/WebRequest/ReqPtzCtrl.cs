using System;
using LibCommon.Enums;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// ptz控制请求结构
    /// </summary>
    [Serializable]
    public class ReqPtzCtrl
    {
        private string? _channelId;
        private string _deviceId;
        private PTZCommandType _ptzCommandType;
        private int _speed;

        /// <summary>
        /// 控制类型
        /// </summary>
        public PTZCommandType PtzCommandType
        {
            get => _ptzCommandType;
            set => _ptzCommandType = value;
        }

        /// <summary>
        /// 移动速度1-254
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