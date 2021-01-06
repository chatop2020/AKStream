using System;

namespace LibCommon.Structs.WebRequest.AKStreamKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReqKeeperCutOrMergeVideoFile
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private string? _mediaServerId;
        private string? _app;
        private string? _vhost;
        private string? _streamId;
        private string? _callbackUrl;

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? CallbackUrl
        {
            get => _callbackUrl;
            set => _callbackUrl = value;
        }
    }
}