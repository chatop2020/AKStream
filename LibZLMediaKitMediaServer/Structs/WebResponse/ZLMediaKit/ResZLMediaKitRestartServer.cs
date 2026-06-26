using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 重启服务器的回复结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitRestartServer : ResZLMediaKitResponseBase
    {
        private string? _msg;

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}