using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    /// <summary>
    /// 停止录制返回结构
    /// </summary>
    [Serializable]
    public class ResZLMediaKitStopRecord : ResZLMediaKitResponseBase
    {
        private bool _result;

        public bool Result
        {
            get => _result;
            set => _result = value;
        }
    }
}