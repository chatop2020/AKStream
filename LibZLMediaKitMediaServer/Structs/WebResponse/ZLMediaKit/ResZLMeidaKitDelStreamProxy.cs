using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ResZLMeidaKitDelStreamProxy : ResZLMediaKitResponseBase
    {
        private Data_1 _data;

        public Data_1 Data
        {
            get => _data;
            set => _data = value;
        }
    }
}