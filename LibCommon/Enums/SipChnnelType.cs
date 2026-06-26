using System;

namespace LibCommon.Enums
{
    /// <summary>
    /// Sip通道类型
    /// </summary>
    [Serializable]
    public enum SipChannelType
    {
        /// <summary>
        /// 音视频流通道
        /// </summary>
        VideoChannel,

        /// <summary>
        /// 报警通道
        /// </summary>
        AlarmChannel,

        /// <summary>
        /// 音频流通道
        /// </summary>
        AudioChannel,

        /// <summary>
        /// 其他通道
        /// </summary>
        OtherChannel,

        /// <summary>
        /// id位数不等于20,设置为未知设备
        /// </summary>
        Unknow,
    }
}