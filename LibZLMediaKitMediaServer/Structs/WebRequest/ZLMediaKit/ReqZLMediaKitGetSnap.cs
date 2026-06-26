using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 截图请求的结构
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitGetSnap : ReqZLMediaKitRequestBase
    {
        private int _expire_sec;
        private int _timeout_sec;
        private string _url;

        /// <summary>
        /// 需要截图的url，可以是本机的，也可以是远程主机的
        /// </summary>
        public string Url
        {
            get => _url;
            set => _url = value;
        }

        /// <summary>
        /// 截图失败超时时间，防止FFmpeg一直等待截图
        /// </summary>
        public int Timeout_Sec
        {
            get => _timeout_sec;
            set => _timeout_sec = value;
        }

        /// <summary>
        /// 截图的过期时间，该时间内产生的截图都会作为缓存返回
        /// </summary>
        public int Expire_Sec
        {
            get => _expire_sec;
            set => _expire_sec = value;
        }
    }
}