using System;
using LibCommon.Enums;
using Newtonsoft.Json;

namespace LibCommon.Structs
{
    /// <summary>
    /// 查询sip录像的结构
    /// </summary>
    [Serializable]
    public class SipQueryRecordFile
    {
        private DateTime _endTime;
        private SipRecordFileQueryType _sipRecordFileQueryType;
        private DateTime _startTime;
        private int? _taskId;

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
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 结束时间
        /// </summary>

        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 任务的taskid,用于回查,外部不用传
        /// </summary>
        [JsonIgnore]
        public int? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }
    }
}