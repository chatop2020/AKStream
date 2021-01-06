using System;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class Data
    {
        private string? _key;

        public string? Key
        {
            get => _key;
            set => _key = value;
        }
    }

    [Serializable]
    public class ResZLMediaKitAddFFmpegProxy : ResZLMediaKitResponseBase
    {
        private Data _data;

        /// <summary>
        /// 返回的数据
        /// </summary>
        public Data Data
        {
            get => _data;
            set => _data = value;
        }
    }
}