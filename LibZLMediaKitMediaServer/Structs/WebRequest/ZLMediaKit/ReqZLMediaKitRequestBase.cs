using System;
using System.Text.Json.Serialization;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitRequestBase
    {
        private string? _secret;

        [JsonIgnore]
        public string? Secret
        {
            get => _secret;
            set => _secret = value;
        }
    }
}