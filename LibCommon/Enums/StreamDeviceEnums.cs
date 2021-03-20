using System;

namespace LibCommon.Enums
{
    /// <summary>
    /// 流设备类型
    /// </summary>
    [Serializable]
    public enum DeviceType
    {
        ONVIF,
        GB28181,
        OTHER,
    }

    /// <summary>
    /// 设备推流状态
    /// </summary>
    [Serializable]
    public enum PushStatus
    {
        /// <summary>
        /// 空闲的
        /// </summary>
        IDLE,

        /// <summary>
        /// 推流中
        /// </summary>
        PUSHON,

        /// <summary>
        /// 忽略，不做处理
        /// </summary>
        IGNORE,
    }
}