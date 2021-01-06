using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitIsRecording : ResZLMediaKitResponseBase
    {
        private bool _status;

        public bool Status
        {
            get => _status;
            set => _status = value;
        }
    }
}