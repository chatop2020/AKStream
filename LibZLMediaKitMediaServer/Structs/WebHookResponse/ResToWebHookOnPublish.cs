using System;
using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse
{
    [Serializable]
    public class ResToWebHookOnPublish : ResZLMediaKitResponseBase
    {
        private bool? _enableHls;
        private bool? _enableMP4;
        private string? _msg;

        public bool? EnableHls
        {
            get => _enableHls;
            set => _enableHls = value;
        }

        public bool? EnableMP4
        {
            get => _enableMP4;
            set => _enableMP4 = value;
        }


        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}