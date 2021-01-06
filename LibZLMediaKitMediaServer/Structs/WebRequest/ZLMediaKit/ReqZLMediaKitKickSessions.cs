using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 踢掉多个session
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitKickSessions : ReqZLMediaKitRequestBase
    {
        private int? _local_port;
        private string? _peer_ip;

        /// <summary>
        /// 本机端口
        /// </summary>
        public int? Local_Port
        {
            get => _local_port;
            set => _local_port = value;
        }

        /// <summary>
        /// 客户端ip地址
        /// </summary>
        public string? Peer_Ip
        {
            get => _peer_ip;
            set => _peer_ip = value;
        }
    }
}