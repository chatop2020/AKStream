using System;
using LibCommon.Enums;

namespace LibCommon.Structs
{
    /// <summary>
    /// 查询sip录像的结构
    /// </summary>
    [Serializable]
    public class SipQueryRecordFile
    {
        private SipRecordFileQueryType _sipRecordFileQueryType;
        private DateTime startTime;
        private DateTime endTime;

        /// <summary>
        /// 查询录像类型，一般为all
        /// </summary>
        public SipRecordFileQueryType SipRecordFileQueryType
        {
            get => _sipRecordFileQueryType;
            set => _sipRecordFileQueryType = value;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get => startTime;
            set => startTime = value;
        }

        /// <summary>
        /// 结束时间
        /// </summary>

        public DateTime EndTime
        {
            get => endTime;
            set => endTime = value;
        }
    }
}