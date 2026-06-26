using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitOpenRtpPort : ResZLMediaKitResponseBase
    {
        private ushort? _port;


        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }
    }
}