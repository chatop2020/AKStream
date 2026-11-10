using System.Collections.Generic;
using AKStreamKeeper.Services;
using LibCommon.Structs;
using Microsoft.AspNetCore.Mvc;

namespace AKStreamKeeper.Controllers
{
    public partial class ApiServiceController
    {
        /// <summary>内部接口：停流前保存端口隔离和原 SIP dialog，沿用控制器 AccessKey 鉴权。</summary>
        /// <param name="AccessKey">Keeper 调用密钥。</param>
        /// <param name="stop">本次停流上下文，不能作为日志正文输出。</param>
        /// <returns>隔离记录已持久化时返回 true。</returns>
        [HttpPost("HoldRtpPortStop")]
        public bool HoldRtpPortStop([FromHeader(Name = "AccessKey")] string AccessKey, RtpPortStop stop)
            => ApiService.HoldRtpPortStop(stop);

        /// <summary>内部接口：合并对应 SIP 会话的清理结果，完成后开始冷却。</summary>
        /// <param name="AccessKey">Keeper 调用密钥。</param>
        /// <param name="stop">会话标识及本次 BYE、接收服务关闭结果。</param>
        /// <returns>当前任务接受该结果时返回 true。</returns>
        [HttpPost("CompleteRtpPortStop")]
        public bool CompleteRtpPortStop([FromHeader(Name = "AccessKey")] string AccessKey, RtpPortStop stop)
            => ApiService.CompleteRtpPortStop(stop);

        /// <summary>内部接口：供 Web 重试尚未完成的停流，记录包含 dialog，禁止外发。</summary>
        /// <param name="AccessKey">Keeper 调用密钥。</param>
        /// <returns>尚未解除隔离的停流上下文。</returns>
        [HttpGet("GetPendingRtpPortStops")]
        public List<RtpPortStop> GetPendingRtpPortStops([FromHeader(Name = "AccessKey")] string AccessKey)
            => ApiService.GetPendingRtpPortStops();
    }
}
