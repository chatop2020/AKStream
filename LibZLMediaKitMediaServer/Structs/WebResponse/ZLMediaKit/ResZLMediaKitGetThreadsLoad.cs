using System;
using System.Collections.Generic;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 细项
    /// </summary>
    [Serializable]
    public class ResZLMediaKitGetThreadsLoadItem
    {
        private int _delay;
        private int _load;

        /// <summary>
        /// 线程延迟
        /// </summary>
        public int Delay
        {
            get => _delay;
            set => _delay = value;
        }

        /// <summary>
        /// 线程负载
        /// </summary>
        public int Load
        {
            get => _load;
            set => _load = value;
        }
    }

    /// <summary>
    /// 获取各epoll(或select)线程负载以及延时的结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitGetThreadsLoad : ResZLMediaKitResponseBase
    {
        private List<ResZLMediaKitGetThreadsLoadItem>? _data;

        /// <summary>
        /// 线程延迟负载信息列表
        /// </summary>
        public List<ResZLMediaKitGetThreadsLoadItem>? Data
        {
            get => _data;
            set => _data = value;
        }
    }
}