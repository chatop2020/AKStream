using System;

namespace LibCommon.Structs.WebRequest
{
    /// <summary>
    /// 请求结构-获取录制计划
    /// </summary>
    [Serializable]
    public class ReqGetRecordPlan
    {
        private string? _mainId;
        private string? _mediaServerId;

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 摄像头实例ID
        /// </summary>
        public string? MainId
        {
            get => _mainId;
            set => _mainId = value;
        }
    }
}