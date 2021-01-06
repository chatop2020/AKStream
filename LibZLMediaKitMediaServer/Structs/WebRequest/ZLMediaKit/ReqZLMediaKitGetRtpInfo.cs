using System;

namespace LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit
{
    /// <summary>
    /// 获取rtp信息请求结构
    /// </summary>
    [Serializable]
    public class ReqZLMediaKitGetRtpInfo : ReqZLMediaKitRequestBase
    {
        private string _stream_id;

        /// <summary>
        /// straem
        /// </summary>
        public string Stream_Id
        {
            get => _stream_id;
            set => _stream_id = value;
        }
    }
}