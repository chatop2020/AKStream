using System;

namespace LibCommon.Structs.WebResponse
{
    /// <summary>
    /// 申请rtp端口返回结构
    /// </summary>
    [Serializable]
    public class ResMediaServerOpenRtpPort
    {
        private ushort _port;
        private string _stream;

        /// <summary>
        /// rtp端口
        /// </summary>
        public ushort Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 绑定的streamid
        /// </summary>
        public string Stream
        {
            get => _stream;
            set => _stream = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}