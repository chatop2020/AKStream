using System;
using System.Collections.Generic;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class Item
    {
        private string _id;
        private string _local_ip;
        private int _local_port;
        private string _peer_ip;
        private int _peer_port;
        private string _typeid;

        /// <summary>
        /// tcp唯一id
        /// </summary>
        public string Id
        {
            get => _id;
            set => _id = value;
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
        /// tcp session 类型
        /// </summary>
        public string Typeid
        {
            get => _typeid;
            set => _typeid = value;
        }
    }


    /// <summary>
    /// TcpSession列表回复结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitGetAllSession : ResZLMediaKitResponseBase
    {
        private List<Item> _data;

        public List<Item> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}