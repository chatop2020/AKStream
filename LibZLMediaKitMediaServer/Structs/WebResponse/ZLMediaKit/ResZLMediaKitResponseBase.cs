using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitResponseBase
    {
        private int _code;
        private string? _msg;

        public int Code
        {
            get => _code;
            set => _code = value;
        }
        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}