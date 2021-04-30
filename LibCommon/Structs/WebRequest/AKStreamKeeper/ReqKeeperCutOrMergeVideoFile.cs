using System;

namespace LibCommon.Structs.WebRequest.AKStreamKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReqKeeperCutOrMergeVideoFile
    {
        private string? _app;
        private string? _callbackUrl;
        private DateTime _endTime;
        private string? _mainId;
        private string? _mediaServerId;
        private DateTime _startTime;
        private string? _vhost;

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
        public string? MainId
        {
            get => _mainId;
            set => _mainId = value;
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