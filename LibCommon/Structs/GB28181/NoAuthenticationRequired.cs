using System;

namespace LibCommon.Structs.GB28181
{
    /// <summary>
    /// 不需要鉴权的设备
    /// </summary>
    [Serializable]
    public class NoAuthenticationRequired
    {
        private string _deviceId;
        private string? _ipV4Address;
        private string? _ipV6Address;

        /// <summary>
        /// 设备ipv4地址(可空)
        /// </summary>
        public string? IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value;
        }

        /// <summary>
        /// 设备ipv6地址(可空)
        /// </summary>

        public string? IpV6Address
        {
            get => _ipV6Address;
            set => _ipV6Address = value;
        }

        /// <summary>
        /// 设备id
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}