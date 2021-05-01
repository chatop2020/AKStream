using System;
using System.Collections.Generic;

namespace LibCommon.Structs.GB28181
{
    [Serializable]
    /// <summary>
    /// sip服务器配置
    /// </summary>
    public class SipServerConfig
    {
        private bool _authentication;
        private string _gbVersion = null!;
        private bool _ipV6Enable = false;
        private int _keepAliveInterval;
        private int _keepAliveLostNumber;
        private string _msgProtocol = null!;
        private List<NoAuthenticationRequired>? _noAuthenticationRequireds = new List<NoAuthenticationRequired>();
        private string _realm;
        private string _serverSipDeviceId = null!;
        private string _sipIpAddress = null!;
        private string? _sipIpV6Address;
        private string? _sipPassword;
        private ushort _sipPort;
        private string? _sipUsername;


        /// <summary>
        /// sip服务器ip地址
        /// </summary>
        public string SipIpAddress
        {
            get => _sipIpAddress;
            set => _sipIpAddress = value;
        }

        /// <summary>
        /// ipv6地址
        /// </summary>
        public string? SipIpV6Address
        {
            get => _sipIpV6Address;
            set => _sipIpV6Address = value;
        }


        /// <summary>
        /// sip服务器id
        /// </summary>
        public string ServerSipDeviceId
        {
            get => _serverSipDeviceId;
            set => _serverSipDeviceId = value;
        }

        /// <summary>
        /// sip服务端口
        /// </summary>
        public ushort SipPort
        {
            get => _sipPort;
            set => _sipPort = value;
        }

        /// <summary>
        /// 服务器域
        /// </summary>
        public string Realm
        {
            get => _realm;
            set => _realm = value;
        }

        /// <summary>
        /// sip执行的版本
        /// </summary>
        public string GbVersion
        {
            get => _gbVersion;
            set => _gbVersion = value;
        }

        /// <summary>
        /// 是否启用认证
        /// </summary>
        public bool Authentication
        {
            get => _authentication;
            set => _authentication = value;
        }

        /// <summary>
        /// sip用户名
        /// </summary>
        public string? SipUsername
        {
            get => _sipUsername;
            set => _sipUsername = value;
        }

        /// <summary>
        /// sip密码
        /// </summary>
        public string? SipPassword
        {
            get => _sipPassword;
            set => _sipPassword = value;
        }

        /// <summary>
        /// sip消息使用的协议
        /// </summary>
        public string MsgProtocol
        {
            get => _msgProtocol;
            set => _msgProtocol = value;
        }

        /// <summary>
        /// 心跳保持周期
        /// </summary>
        public int KeepAliveInterval
        {
            get => _keepAliveInterval;
            set => _keepAliveInterval = value;
        }

        /// <summary>
        /// 多少次心跳丢失后算该设备下线
        /// </summary>
        public int KeepAliveLostNumber
        {
            get => _keepAliveLostNumber;
            set => _keepAliveLostNumber = value;
        }

        /// <summary>
        /// ipv6使能
        /// </summary>
        public bool IpV6Enable
        {
            get => _ipV6Enable;
            set => _ipV6Enable = value;
        }

        /// <summary>
        /// 无需鉴权的设备(可空)
        /// </summary>
        public List<NoAuthenticationRequired>? NoAuthenticationRequireds
        {
            get => _noAuthenticationRequireds;
            set => _noAuthenticationRequireds = value;
        }
    }
}