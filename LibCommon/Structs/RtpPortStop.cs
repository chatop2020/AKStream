using System;

namespace LibCommon.Structs
{
    /// <summary>
    /// 一次 RTP 停流清理的持久化记录。由 Keeper 保存，只有同一 StopId 的 BYE 和接收端口
    /// 均确认完成后才开始冷却；信令原文仅用于内部重试，不应写入普通日志。
    /// </summary>
    public class RtpPortStop
    {
        /// <summary>本地 RTP 接收端口，不包含相邻的 RTCP 端口。</summary>
        public ushort Port { get; set; }
        /// <summary>停流所属 SIP Call-ID，用于拒绝旧清理任务解除其他会话的隔离。</summary>
        public string StopId { get; set; } = "";
        /// <summary>端口所在的媒体服务器。</summary>
        public string MediaServerId { get; set; } = "";
        /// <summary>设备国标编号。</summary>
        public string DeviceId { get; set; } = "";
        /// <summary>通道国标编号。</summary>
        public string ChannelId { get; set; } = "";
        /// <summary>需要关闭的 ZLM 流标识。</summary>
        public string Stream { get; set; } = "";
        /// <summary>需要关闭的应用名。</summary>
        public string App { get; set; } = "rtp";
        /// <summary>需要关闭的虚拟主机。</summary>
        public string Vhost { get; set; } = "__defaultVhost__";
        /// <summary>原 INVITE 请求，重试必须继续使用其 Call-ID 和 From tag。</summary>
        public string InviteRequest { get; set; } = "";
        /// <summary>原 INVITE 成功响应，重试必须继续使用其 To tag 和 CSeq。</summary>
        public string InviteResponse { get; set; } = "";
        /// <summary>是否为设备录像回放会话。</summary>
        public bool Playback { get; set; }
        /// <summary>本次等待 BYE 应答的毫秒数，沿用 Web 的 WaitSipRequestTimeOutMSec。</summary>
        public int ByeTimeoutMilliseconds { get; set; } = 5000;
        /// <summary>是否已收到对应 BYE 的成功应答。</summary>
        public bool ByeConfirmed { get; set; }
        /// <summary>是否已确认本地 RTP 接收服务不存在。</summary>
        public bool ReceiverClosed { get; set; }
        /// <summary>已执行的清理次数，自动重试最多执行四次，仍失败时保留隔离供人工重试。</summary>
        public int Attempts { get; set; }
        /// <summary>下一次自动重试的 UTC 时间。</summary>
        public DateTime NextRetryAt { get; set; }
        /// <summary>两侧清理完成后的 UTC 冷却截止时间；空值表示仍禁止复用。</summary>
        public DateTime? ReuseAfter { get; set; }
        /// <summary>最近一次清理失败原因，不包含完整 SIP 报文。</summary>
        public string LastError { get; set; } = "";
    }
}
