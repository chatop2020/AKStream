using System;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse
{
    [Serializable]
    public class ResToWebHookOnPlay : ResZLMediaKitResponseBase
    {
        private string? _msg;

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}