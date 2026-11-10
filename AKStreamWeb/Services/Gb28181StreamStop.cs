using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.XML;
using LibCommon.Structs.GB28181.Sys;
using LibCommon.Structs.Other;
using LibGB28181SipServer;
using LibZLMediaKitMediaServer;
using LibZLMediaKitMediaServer.Structs.WebRequest.ZLMediaKit;
using SIPSorcery.SIP;

namespace AKStreamWeb.Services
{
    /// <summary>
    /// 协调 BYE、ZLM 本地关流和 Keeper 端口隔离。Keeper 保存唯一持久化任务；本类只保留
    /// 正在执行的短期互斥，避免关流触发的回调再次关闭新流或重复发送 BYE。
    /// </summary>
    public static class Gb28181StreamStop
    {
        private static readonly ConcurrentDictionary<string, byte> Running = new ConcurrentDictionary<string, byte>();
        private static int _retryRunning;

        /// <summary>判断某条流是否正在清理，回调此时只更新在线记录，不重复释放端口。</summary>
        /// <param name="serverId">目标媒体服务器。</param>
        /// <param name="stream">ZLM 流标识。</param>
        public static bool IsRunning(string serverId, string stream) => Running.ContainsKey(serverId + ":" + stream);

        /// <summary>隔离接收端口后等待 BYE，随后无论应答是否成功都尝试关闭本地接收服务。</summary>
        /// <param name="server">拥有接收端口的媒体服务器。</param>
        /// <param name="channel">原直播或回放 dialog 的通道副本。</param>
        /// <param name="port">接收端口，零表示未找到有效端口，此时不能假定已经停止。</param>
        /// <param name="stream">待关闭的流标识。</param>
        /// <param name="app">流应用名。</param>
        /// <param name="vhost">流虚拟主机。</param>
        /// <param name="playback">是否是回放。</param>
        /// <param name="rs">两侧清理未完成时返回明确失败，不被后续成功覆盖。</param>
        /// <param name="timeoutMs">本次 BYE 应答等待时限，毫秒；持久化后用于同一任务重试。</param>
        /// <param name="receiverAbsentConfirmed">端口未知时，是否已经成功查询并确认没有该流的接收服务。</param>
        /// <returns>BYE 和接收端口关闭均确认，且结果已保存时返回 true。</returns>
        public static bool Stop(ServerInstance server, SipChannel channel, ushort port, string stream,
            string app, string vhost, bool playback, out ResponseStruct rs, int timeoutMs = 5000,
            bool receiverAbsentConfirmed = false)
        {
            var pending = server.KeeperWebApi.GetPendingRtpPortStops(out rs);
            if (pending == null || rs.Code != ErrorNumber.None) return false;
            var previous = pending.FirstOrDefault(x => x.MediaServerId == server.MediaServerId &&
                x.DeviceId == channel.ParentId && x.ChannelId == channel.DeviceId && x.Stream == stream);
            if (previous != null) return Execute(server, previous, out rs);

            if (port == 0)
            {
                if (channel.PushStatus == PushStatus.IDLE && receiverAbsentConfirmed)
                {
                    rs = new ResponseStruct { Code = ErrorNumber.None, Message = "没有未完成停流任务和接收端口。" };
                    return true;
                }
                rs = Failure("未找到有效接收端口或完整 SIP dialog，不能确认设备已停止。");
                return false;
            }
            var stop = new RtpPortStop
            {
                Port = port, StopId = channel.InviteSipRequest?.Header.CallId ?? "missing-dialog-" + Guid.NewGuid().ToString("N"),
                MediaServerId = server.MediaServerId, DeviceId = channel.ParentId, ChannelId = channel.DeviceId,
                Stream = stream, App = app ?? "rtp", Vhost = vhost ?? "__defaultVhost__", Playback = playback,
                ByeTimeoutMilliseconds = Math.Max(1, timeoutMs),
                InviteRequest = channel.InviteSipRequest?.ToString() ?? "", InviteResponse = channel.InviteSipResponse?.ToString() ?? ""
            };
            if (!server.KeeperWebApi.HoldRtpPortStop(stop, out rs))
            {
                if (rs.Code == ErrorNumber.None) rs = Failure("接收端口隔离失败或仍属于另一清理会话。");
                return false;
            }
            return Execute(server, stop, out rs);
        }

        /// <summary>共享固定入口不属于单条流，停止时只关闭对应流，不能关闭公共监听或隔离整个入口。</summary>
        /// <param name="server">目标媒体服务器。</param>
        /// <param name="channel">需要结束的 SIP dialog。</param>
        /// <param name="stream">仅此流需要关闭。</param>
        /// <param name="app">流应用名。</param>
        /// <param name="vhost">流虚拟主机。</param>
        /// <param name="rs">保留 BYE 失败或本地关流失败原因。</param>
        /// <param name="timeoutMs">本次 BYE 等待毫秒数。</param>
        /// <returns>设备应答和本地关流均成功时返回 true。</returns>
        public static bool StopShared(ServerInstance server, SipChannel channel, string stream, string app,
            string vhost, out ResponseStruct rs, int timeoutMs = 5000)
        {
            var bye = false;
            var byeResult = Failure("共享入口流的 SIP dialog 未确认。");
            try
            {
                using var proxy = new SipMethodProxy(Math.Max(1, timeoutMs));
                bye = proxy.DeInvite(channel, out byeResult);
            }
            catch (Exception ex) { byeResult = Failure("BYE 清理失败：" + ex.Message); }
            try
            {
                var closed = server.WebApiHelper.CloseStreams(new ReqZLMediaKitCloseStreams
                    { App = app, Vhost = vhost, Stream = stream, Force = true }, out var closeResult);
                rs = !bye ? byeResult : closeResult;
                return bye && closed != null && closed.Code == 0 && closeResult.Code == ErrorNumber.None;
            }
            catch (Exception ex) { rs = !bye ? byeResult : Failure("本地关流失败：" + ex.Message); return false; }
        }

        /// <summary>在原有维护循环中触发有界重试，不阻塞维护线程；重启后从 Keeper 恢复任务。</summary>
        /// <param name="servers">本轮维护时捕获的媒体节点快照。</param>
        public static void ScheduleRetry(IEnumerable<ServerInstance> servers)
        {
            if (Interlocked.CompareExchange(ref _retryRunning, 1, 0) != 0) return;
            var snapshot = servers.ToArray();
            Task.Run(() =>
            {
                try
                {
                    foreach (var server in snapshot)
                    {
                        if (server == null || !server.IsKeeperRunning) continue;
                        var pending = server.KeeperWebApi.GetPendingRtpPortStops(out _);
                        if (pending == null) continue;
                        foreach (var stop in pending.Where(x => x.MediaServerId == server.MediaServerId &&
                                     x.Attempts < 4 && x.NextRetryAt <= DateTime.UtcNow))
                            Execute(server, stop, out _);
                    }
                }
                catch (Exception ex) { GCommon.Logger.Warn("停流清理重试失败：" + ex.Message); }
                finally { Interlocked.Exchange(ref _retryRunning, 0); }
            });
        }

        /// <summary>重试始终使用原 dialog，不读取可能已经被新 INVITE 覆盖的会话标识。</summary>
        private static bool Execute(ServerInstance server, RtpPortStop stop, out ResponseStruct rs)
        {
            rs = Failure("此流正在执行停流清理。");
            var key = server.MediaServerId + ":" + stop.Stream;
            if (!Running.TryAdd(key, 0)) return false;
            try
            {
                lock (SipServerService.GetLiveLock(server.MediaServerId, stop.Stream))
                {
                if (!stop.ByeConfirmed)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(stop.InviteRequest) || string.IsNullOrWhiteSpace(stop.InviteResponse))
                            throw new InvalidOperationException("原 SIP dialog 不完整，保持端口隔离，需确认设备清理后处理。");
                        var response = SIPResponse.ParseSIPResponse(stop.InviteResponse);
                        if (response.Header.CallId != stop.StopId)
                            throw new InvalidOperationException("原 SIP dialog 标识不一致，保持接收端口隔离。");
                        response.Header.CSeq += stop.Attempts;
                        var channel = new SipChannel
                        {
                            ParentId = stop.DeviceId, DeviceId = stop.ChannelId, Stream = stop.Stream,
                            SipChannelStatus = DevStatus.ON, SipChannelType = SipChannelType.VideoChannel,
                            PushStatus = PushStatus.IGNORE,
                            InviteSipRequest = SIPRequest.ParseSIPRequest(stop.InviteRequest), InviteSipResponse = response
                        };
                        using var proxy = new SipMethodProxy(Math.Max(1, stop.ByeTimeoutMilliseconds));
                        stop.ByeConfirmed = proxy.DeInvite(channel, out var byeResult) && byeResult.Code == ErrorNumber.None;
                        stop.LastError = stop.ByeConfirmed ? "" : byeResult.Message;
                    }
                    catch (Exception ex) { stop.LastError = "BYE 清理失败：" + ex.Message; }
                }

                // BYE 超时也必须关本地流；先前的隔离已经落盘，回调和自动清理不能把端口放回池中。
                try
                {
                    var before = server.WebApiHelper.ListRtpServer(new ReqZLMediaKitRequestBase(), out var beforeResult);
                    if (beforeResult.Code != ErrorNumber.None || before?.Code != 0 || before.Data == null)
                        throw new InvalidOperationException("无法查询当前接收服务，不能确认端口归属。");
                    // 同一流名可能已经由后来一次 INVITE 使用新端口，旧清理只能处理原端口。
                    var newer = before.Data.Any(x => x.Stream_Id == stop.Stream && x.Port != stop.Port);
                    if (!newer)
                    {
                        server.WebApiHelper.CloseStreams(new ReqZLMediaKitCloseStreams
                            { App = stop.App, Vhost = stop.Vhost, Stream = stop.Stream, Force = true }, out _);
                        server.WebApiHelper.CloseRtpPort(new ReqZLMediaKitCloseRtpPort { Stream_Id = stop.Stream }, out _);
                    }
                    var list = server.WebApiHelper.ListRtpServer(new ReqZLMediaKitRequestBase(), out var listResult);
                    stop.ReceiverClosed = listResult.Code == ErrorNumber.None && list != null && list.Code == 0 &&
                        list.Data != null && !list.Data.Any(x => x.Port == stop.Port || !newer && x.Stream_Id == stop.Stream);
                    if (!stop.ReceiverClosed) stop.LastError += "；本地 RTP 接收端口关闭未确认。";
                }
                catch (Exception ex) { stop.ReceiverClosed = false; stop.LastError += "；本地关闭失败：" + ex.Message; }

                var saved = server.KeeperWebApi.CompleteRtpPortStop(stop, out var keeperResult);
                var success = stop.ByeConfirmed && stop.ReceiverClosed && saved && keeperResult.Code == ErrorNumber.None;
                if (success)
                {
                    // 只更新原 Call-ID 对应的状态，不能清空后来创建的 dialog 或直播通道。
                    var original = LibGB28181SipServer.Common.SipDevices.FindLast(x => x.DeviceId == stop.DeviceId)?
                        .SipChannels.FindLast(x => x.DeviceId == stop.ChannelId && x.InviteSipRequest?.Header.CallId == stop.StopId);
                    if (original != null) { original.PushStatus = PushStatus.IDLE; original.RtpPort = null; }
                    foreach (var query in GCommon.VideoChannelRecordInfo.ToArray())
                        foreach (var item in (query.RecItems ?? new List<RecordInfo.RecItem>()).Where(x => x.InviteSipRequest?.Header.CallId == stop.StopId))
                            item.PushStatus = PushStatus.IDLE;
                }
                rs = success ? new ResponseStruct { Code = ErrorNumber.None, Message = "设备和本地接收均已停止，端口进入冷却。" }
                    : Failure($"停流未完全确认，接收端口 {stop.Port} 保持隔离。{stop.LastError}");
                GCommon.Logger.Info($"停流清理结果->{server.MediaServerId}->{stop.Stream}->Port:{stop.Port}->BYE:{stop.ByeConfirmed}->RTP关闭:{stop.ReceiverClosed}->保存:{saved}");
                return success;
                }
            }
            finally { Running.TryRemove(key, out _); }
        }

        /// <summary>生成停流未确认结果，避免把本地成功误报为设备侧已经停止。</summary>
        private static ResponseStruct Failure(string message) =>
            new ResponseStruct { Code = ErrorNumber.Sip_DeInviteExcept, Message = message };
    }
}
