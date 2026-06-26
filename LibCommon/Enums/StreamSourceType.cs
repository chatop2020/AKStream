using System;

namespace LibCommon.Enums
{
    [Serializable]
    public enum StreamSourceType
    {
        /// <summary>
        /// 直播流
        /// </summary>
        Live,

        /// <summary>
        /// 回放流
        /// </summary>
        PlayBack,

        /// <summary>
        /// 全部
        /// </summary>
        ALL,
    }
}