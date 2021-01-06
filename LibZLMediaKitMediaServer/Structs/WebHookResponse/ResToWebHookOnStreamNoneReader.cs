using System;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse
{
    [Serializable]
    public class ResToWebHookOnStreamNoneReader : ResZLMediaKitResponseBase
    {
        private bool? close;

        public bool? Close
        {
            get => close;
            set => close = value;
        }
    }
}