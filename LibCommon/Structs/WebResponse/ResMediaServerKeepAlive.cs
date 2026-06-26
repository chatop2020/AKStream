using System;

namespace LibCommon.Structs.WebResponse
{
    [Serializable]
    public class ResMediaServerKeepAlive : ResAKStreamWebResponseBase
    {
        private bool _needRestartMediaServer = false;
        private bool _recommendTimeSynchronization = false;
        private DateTime _serverDateTime;

        /// <summary>
        /// 是否建议同步时间
        /// </summary>
        public bool RecommendTimeSynchronization
        {
            get => _recommendTimeSynchronization;
            set => _recommendTimeSynchronization = value;
        }

        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime ServerDateTime
        {
            get => _serverDateTime;
            set => _serverDateTime = value;
        }

        /// <summary>
        /// 要求重启MediaServer
        /// </summary>
        public bool NeedRestartMediaServer
        {
            get => _needRestartMediaServer;
            set => _needRestartMediaServer = value;
        }
    }
}