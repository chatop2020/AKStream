using System;
using LibCommon.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.GB28181
{
    /// <summary>
    /// PTZ控制结构
    /// </summary>
    [Serializable]
    public class PtzCtrl
    {
        private PTZCommandType _ptzCommandType;
        private SipChannel? _sipChannel = null;
        private SipDevice _sipDevice = null;
        private int _speed;

        /// <summary>
        /// 控制命令类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PTZCommandType PtzCommandType
        {
            get => _ptzCommandType;
            set => _ptzCommandType = value;
        }

        /// <summary>
        /// 速度
        /// </summary>
        public int Speed
        {
            get => _speed;
            set
            {
                if (value > 255)
                {
                    _speed = 255;
                }
                else if (value < 0)
                {
                    _speed = 0;
                }
                else
                {
                    _speed = value;
                }
            }
        }

        /// <summary>
        /// 控制设备
        /// </summary>
        public SipDevice SipDevice
        {
            get => _sipDevice;
            set => _sipDevice = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 控制通道
        /// </summary>
        public SipChannel? SipChannel
        {
            get => _sipChannel;
            set => _sipChannel = value;
        }
    }
}