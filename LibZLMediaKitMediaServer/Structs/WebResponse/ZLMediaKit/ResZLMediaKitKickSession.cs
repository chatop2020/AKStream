using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 踢掉session的返回结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitKickSession : ResZLMediaKitResponseBase
    {
        private string _msg;

        public string Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}