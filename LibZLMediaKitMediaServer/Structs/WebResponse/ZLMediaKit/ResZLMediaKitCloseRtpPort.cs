using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitCloseRtpPort : ResZLMediaKitResponseBase
    {
        private int? _hit;

        public int? Hit
        {
            get => _hit;
            set => _hit = value;
        }
    }
}