using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediakitStartSendRtp : ResZLMediaKitResponseBase
    {
        private string _local_port;
        private string? _msg;

        public string Local_Port
        {
            get => _local_port;
            set => _local_port = value;
        }

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}