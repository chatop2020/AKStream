using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMediaKitResponseBase
    {
        private int _code;

        public int Code
        {
            get => _code;
            set => _code = value;
        }
    }
}