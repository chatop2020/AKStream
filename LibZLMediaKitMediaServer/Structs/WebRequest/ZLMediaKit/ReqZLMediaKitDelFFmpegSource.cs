using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitDelFFmpegSource : ReqZLMediaKitRequestBase
    {
        private string? _key;

        public string? Key
        {
            get => _key;
            set => _key = value;
        }
    }
}