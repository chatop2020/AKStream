using System;
using LibCommon.Structs.WebRequest.AKStreamKeeper;

namespace LibCommon.Structs.WebResponse.AKStreamKeeper
{
    [Serializable]
    public class ResKeeperCutMergeTaskStatusResponse
    {
        private string? _callbakUrl;
        private DateTime _createTime;
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

        public string? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }

        public string? CallbakUrl
        {
            get => _callbakUrl;
            set => _callbakUrl = value;
        }

        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        public MyTaskStatus? TaskStatus
        {
            get => _taskStatus;
            set => _taskStatus = value;
        }

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