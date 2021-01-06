using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 踢掉session的请求结构
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitKickSession : ReqZLMediaKitRequestBase
    {
        private string _id;

        public string Id
        {
            get => _id;
            set => _id = value;
        }
    }
}