using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibCommon.Structs.WebRequest.AKStreamKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum MyTaskStatus
    {
        Create,
        Packaging,
        Cutting,
        Mergeing,
        Closed,
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReqKeeperCutMergeTask
    {
        private string? _callbakUrl;
        private DateTime _createTime;
        private List<CutMergeStruct>? _cutMergeFileList;
        private string? _playUrl;

        /// <summary>
        /// Create=0%
        /// Packageing=45%
        /// Cutting=15%
        /// Mergeing=40%
        /// </summary>
        private double? _processPercentage = 0f;

        private string? _taskId;
        private MyTaskStatus? _taskStatus;

        /// <summary>
        /// 
        /// </summary>
        public List<CutMergeStruct>? CutMergeFileList
        {
            get => _cutMergeFileList;
            set => _cutMergeFileList = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? CallbakUrl
        {
            get => _callbakUrl;
            set => _callbakUrl = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MyTaskStatus? TaskStatus
        {
            get => _taskStatus;
            set => _taskStatus = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public double? ProcessPercentage
        {
            get => _processPercentage;
            set => _processPercentage = value;
        }

        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }
    }
}