using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 请求开始录制的回复结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitStartRecord : ResZLMediaKitResponseBase
    {
        private bool _result;

        public bool Result
        {
            get => _result;
            set => _result = value;
        }
    }
}