using System;

namespace AKStreamWeb.WebRequest
{
    /// <summary>
    /// 请求sip设备列表结构
    /// </summary>
    [Serializable]
    public class ReqGetSipDeviceList
    {
        private string? _deviceId;
        private string? _ipV4Address;
        private bool? _isReday;
        private int? _port;

        /// <summary>
        /// 设备id
        /// </summary>
        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        /// <summary>
        /// 设备ipv4地址
        /// </summary>
        public string? IpV4Address
        {
            get => _ipV4Address;
            set => _ipV4Address = value;
        }

        /// <summary>
        /// 设备端口
        /// </summary>
        public int? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 设备是否就绪
        /// </summary>
        public bool? IsReday
        {
            get => _isReday;
            set => _isReday = value;
        }
    }
}