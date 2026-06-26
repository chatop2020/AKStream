using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    [Serializable]
    public class ReqZLMediaKitCloseRtpPort : ReqZLMediaKitRequestBase
    {
        private string? _stream_id;

        public string? Stream_Id
        {
            get => _stream_id;
            set => _stream_id = value;
        }
    }
}