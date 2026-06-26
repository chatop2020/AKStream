using System;

namespace LibCommon.Enums
{
    /// <summary>
    /// 查询sip录像时的录像文件类型
    /// </summary>
    [Serializable]
    public enum SipRecordFileQueryType
    {
        time,
        alarm,
        manual,
        all, //一般用这个就可以了
    }
}