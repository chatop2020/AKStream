using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitGetRtpInfo : ResZLMediaKitResponseBase
    {
        private bool _exist;
        private string _local_ip;
        private int _local_port;
        private string _peer_ip;
        private int _peer_port;

        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exist
        {
            get => _exist;
            set => _exist = value;
        }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string Peer_Ip
        {
            get => _peer_ip;
            set => _peer_ip = value;
        }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public int Peer_Port
        {
            get => _peer_port;
            set => _peer_port = value;
        }

        /// <summary>
        /// 本机ip
        /// </summary>
        public string Local_Ip
        {
            get => _local_ip;
            set => _local_ip = value;
        }

        /// <summary>
        /// 本机端口
        /// </summary>
        public int Local_Port
        {
            get => _local_port;
            set => _local_port = value;
        }
    }
}