using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using AKStreamKeeper.Services;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Enums;
using LibCommon.Structs;
using LibCommon.Structs.GB28181;
using LibCommon.Structs.GB28181.Sys;
using LibGB28181SipServer;
using LibZLMediaKitMediaServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SIPSorcery.SIP;

/// <summary>使用独立工作目录、回环 HTTP/SIP 和真实停流代码验证故障，不访问现场服务或数据库。</summary>
internal static class Program
{
    private static int _checks;
    private static readonly Dictionary<ushort, string> Receivers = new();
    private static bool _closeFails;
    private static int _replyMode;

    /// <summary>运行持久化、实际 BYE 超时和接收端口关闭故障回归；失败以非零退出码结束。</summary>
    public static async Task Main()
    {
        var directory = Path.Combine(Path.GetTempPath(), "akstream-rtp-stop-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Environment.CurrentDirectory = directory;
        Directory.CreateDirectory("Config");
        File.WriteAllText("Config/logconfig.xml", "<log4net><root><level value=\"OFF\" /></root></log4net>");
        GCommon.InitLogger();
        // Keeper 的历史静态初始化会启动前台性能采样线程；隔离测试不运行采样，避免测试结束后挂住。
        typeof(LibSystemInfo.SystemInfo).GetField("_abort", System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.NonPublic)!.SetValue(null, true);
        AKStreamKeeper.Common.AkStreamKeeperConfig = new AKStreamKeeper.Misc.AKStreamKeeperConfig { RtpPortCdTime = 60 };
        StoreChecks(directory);
        ThreadPool.SetMinThreads(8, 8);

        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        await using var app = builder.Build();
        app.MapGet("/ApiService/GetPendingRtpPortStops", () => ApiService.GetPendingRtpPortStops());
        // 与 Keeper 的 MVC 入口一样使用 Newtonsoft，支持既有 yyyy-MM-dd HH:mm:ss 日期契约。
        app.MapPost("/ApiService/HoldRtpPortStop", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
            ApiService.HoldRtpPortStop(JsonConvert.DeserializeObject<RtpPortStop>(await new StreamReader(request.Body).ReadToEndAsync())!));
        app.MapPost("/ApiService/CompleteRtpPortStop", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
            ApiService.CompleteRtpPortStop(JsonConvert.DeserializeObject<RtpPortStop>(await new StreamReader(request.Body).ReadToEndAsync())!));
        app.MapPost("/index/api/close_streams", () => new { code = 0, count_closed = 1 });
        app.MapPost("/index/api/closeRtpServer", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(await new StreamReader(request.Body).ReadToEndAsync())!;
            var stream = input.First(x => string.Equals(x.Key, "Stream_Id", StringComparison.OrdinalIgnoreCase)).Value.ToString();
            if (!_closeFails)
                foreach (var port in Receivers.Where(x => x.Value == stream).Select(x => x.Key).ToArray()) Receivers.Remove(port);
            return new { code = _closeFails ? -1 : 0, hit = _closeFails ? 0 : 1 };
        });
        app.MapPost("/index/api/listRtpServer", () => new { code = 0, data = Receivers.Select(x => new { port = x.Key, stream_id = x.Value }).ToArray() });
        await app.StartAsync();
        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        var httpPort = (ushort)new Uri(address).Port;
        using (var warmup = new HttpClient()) await warmup.GetStringAsync(address + "/ApiService/GetPendingRtpPortStops");
        using var server = new ServerInstance { MediaServerId = "test", KeeperWebApi = new KeeperWebApi("127.0.0.1", httpPort, "test"),
            WebApiHelper = new WebApiHelper("127.0.0.1", httpPort, "test") };

        using var peer = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var remotePort = ((IPEndPoint)peer.Client.LocalEndPoint!).Port;
        using var transport = new SIPTransport();
        var local = new SIPUDPChannel(new IPEndPoint(IPAddress.Loopback, 0));
        transport.AddSIPChannel(local);
        transport.SIPTransportResponseReceived += SipMsgProcess.SipTransportResponseReceived;
        var sip = (SipServer)FormatterServices.GetUninitializedObject(typeof(SipServer));
        sip.SipTransport = transport;
        LibGB28181SipServer.Common.SipServer = sip;
        LibGB28181SipServer.Common.SipServerConfig = new SipServerConfig
            { SipIpAddress = "127.0.0.1", SipPort = (ushort)local.Port, ServerSipDeviceId = "34020000002000000001" };
        var device = new SipDevice { DeviceId = "device", RemoteEndPoint = SIPEndPoint.ParseSIPEndPoint($"udp:127.0.0.1:{remotePort}"), SipChannelLayout = local };
        device.SipChannels.Add(new SipChannel { DeviceId = "channel", ParentId = "device", SipChannelType = SipChannelType.VideoChannel, SipChannelStatus = DevStatus.ON });
        LibGB28181SipServer.Common.SipDevices.Add(device);
        using var cancel = new CancellationTokenSource();
        var responder = RespondAsync(peer, cancel.Token);

        var channel = Dialog(device, local.Port, remotePort, "timeout");
        Receivers[62000] = "timeout";
        Check(!Gb28181StreamStop.Stop(server, channel, 62000, "timeout", "rtp", "__defaultVhost__", false, out var timeout, 150), "BYE 不应答时明确失败");
        Check(timeout.Code != ErrorNumber.None && !Receivers.ContainsKey(62000), "BYE 超时仍强制关闭本地接收");
        Check(!ApiService.ReleaseRtpPort(62000), "普通释放不能绕过异常隔离");
        AKStreamKeeper.Common.PortInfoList.Add(new PortInfo { Port = 62000, Useed = true, DateTime = DateTime.Now.AddHours(-2) });
        ApiService.CleanUnusedRtpPorts(new List<ushort>());
        Check(AKStreamKeeper.Common.PortInfoList.Single(x => x.Port == 62000).Useed, "空端口列表自动清理不能解除异常隔离");
        ApiService.CleanUnusedRtpPorts(new List<ushort> { 62002 });
        Check(!AKStreamKeeper.Common.RtpPortStops.CanReuse(62000, DateTime.UtcNow), "非空端口列表自动清理不能解除异常隔离");
        Check(ApiService.GuessAnRtpPort(out _, 62000, 62000) == 0, "隔离端口不能再次分配");
        AKStreamKeeper.Common.PortInfoList.Clear();
        Check(ApiService.GuessAnRtpPort(out _, 62000, 62000) == 0, "重启丢失内存占用后仍不能分配隔离端口");
        Check(new RtpPortStopStore(Path.Combine(directory, "Config/RtpPortStops.json")).Pending().Any(x => x.Port == 62000), "重建存储后隔离仍存在");

        _replyMode = 1;
        Check(Gb28181StreamStop.Stop(server, channel, 62000, "timeout", "rtp", "__defaultVhost__", false, out _, 150), "同一原 dialog 重试成功后完成清理");
        Check(ApiService.GetPendingRtpPortStops().All(x => x.Port != 62000), "成功任务不再重试");
        Check(!AKStreamKeeper.Common.RtpPortStops.CanReuse(62000, DateTime.UtcNow), "成功后仍需等待 CdTime");

        _closeFails = true;
        channel = Dialog(device, local.Port, remotePort, "close-failure");
        Receivers[62002] = "close-failure";
        Check(!Gb28181StreamStop.Stop(server, channel, 62002, "close-failure", "rtp", "__defaultVhost__", false, out _, 150), "BYE 成功但接收服务关闭失败不能报告成功");
        Check(!ApiService.ReleaseRtpPort(62002), "接收服务关闭失败不能回池");
        _closeFails = false;
        Check(Gb28181StreamStop.Stop(server, channel, 62002, "close-failure", "rtp", "__defaultVhost__", false, out _, 150), "保留已经确认的 BYE，重试本地关闭");

        _replyMode = 2;
        channel = Dialog(device, local.Port, remotePort, "wrong-reply");
        Receivers[62004] = "wrong-reply";
        Check(!Gb28181StreamStop.Stop(server, channel, 62004, "wrong-reply", "rtp", "__defaultVhost__", false, out _, 150), "同 Call-ID 的 INVITE 应答不能冒充 BYE 成功");
        _replyMode = 1;
        Check(Gb28181StreamStop.Stop(server, channel, 62004, "wrong-reply", "rtp", "__defaultVhost__", false, out _, 150), "错误应答后原会话仍可重试");
        _replyMode = 3;
        channel = Dialog(device, local.Port, remotePort, "wrong-cseq");
        Receivers[62006] = "wrong-cseq";
        Check(!Gb28181StreamStop.Stop(server, channel, 62006, "wrong-cseq", "rtp", "__defaultVhost__", false, out _, 150), "旧 CSeq 的 BYE 应答不能确认新请求");
        _replyMode = 1;
        Check(Gb28181StreamStop.Stop(server, channel, 62006, "wrong-cseq", "rtp", "__defaultVhost__", false, out _, 150), "旧 CSeq 应答后仍可重试成功");
        Receivers[62008] = "missing-dialog";
        Check(!Gb28181StreamStop.Stop(server, new SipChannel { ParentId = "device", DeviceId = "channel" }, 62008,
            "missing-dialog", "rtp", "__defaultVhost__", false, out _, 150) && !Receivers.ContainsKey(62008), "dialog 缺失仍关闭本地接收并保留隔离");
        Check(AKStreamKeeper.Common.RtpPortStops.PruneExpired(DateTime.UtcNow.AddMinutes(2)) == 4, "冷却结束后清除已完成记录");
        Check(AKStreamKeeper.Common.RtpPortStops.Pending().Single().Port == 62008, "定期清理不删除尚未确认的失败记录");
        Check(!File.ReadAllText("Config/RtpPortStops.json").Contains("INVITE sip:"), "完成后不残留 SIP 报文");
        channel = Dialog(device, local.Port, remotePort, "old-generation");
        Receivers[62012] = "old-generation";
        Check(Gb28181StreamStop.Stop(server, channel, 62010, "old-generation", "rtp", "__defaultVhost__", false, out _, 150),
            "旧端口已经关闭时可完成原 dialog 清理");
        Check(Receivers.ContainsKey(62012), "旧任务不能关闭同名流后来使用的新端口");
        AKStreamKeeper.Common.PortInfoList.Add(new PortInfo { Port = 63000, Useed = true, DateTime = DateTime.Now.AddHours(-2) });
        var releasedAt = DateTime.Now;
        ApiService.CleanUnusedRtpPorts(new List<ushort>());
        Check(!AKStreamKeeper.Common.PortInfoList.Single().Useed && AKStreamKeeper.Common.PortInfoList.Single().DateTime >= releasedAt,
            "正常空列表回收从释放时重置 CdTime");
        Check(ApiService.GuessAnRtpPort(out _, 63000, 63000) == 0, "正常回收后的端口也需等待冷却");
        cancel.Cancel();
        peer.Close();
        try { await responder; } catch (OperationCanceledException) { }
        await app.StopAsync();
        Console.WriteLine($"PASS {_checks} checks; isolated_directory={directory}");
    }

    /// <summary>验证持久化状态本身的失败关闭、会话归属、正常冷却和记录清理规则。</summary>
    private static void StoreChecks(string directory)
    {
        var path = Path.Combine(directory, "state.json");
        var store = new RtpPortStopStore(path);
        var stop = new RtpPortStop { Port = 60000, StopId = "original", InviteRequest = "private-dialog" };
        Check(store.Hold(stop), "首次隔离可保存");
        Check(store.Hold(stop), "同一隔离重复提交幂等");
        Check(!store.Hold(new RtpPortStop { Port = 60000, StopId = "other" }), "其他会话不能覆盖隔离");
        Check(!new RtpPortStopStore(path).CanReuse(60000, DateTime.UtcNow.AddYears(1)), "未确认隔离不会因时间或重启失效");
        Check(!store.Complete(new RtpPortStop { Port = 60000, StopId = "other", ByeConfirmed = true, ReceiverClosed = true }, 60), "过期任务不能解除隔离");
        stop.ByeConfirmed = true;
        store.Complete(stop, 60);
        Check(!store.CanReuse(60000, DateTime.UtcNow.AddMinutes(2)), "只有 BYE 成功仍不能复用");
        stop.ReceiverClosed = true;
        store.Complete(stop, 60);
        Check(!store.CanReuse(60000, DateTime.UtcNow), "完成时开始冷却");
        Check(store.CanReuse(60000, DateTime.UtcNow.AddMinutes(2)), "冷却结束可以复用");
        Check(!File.ReadAllText(path).Contains("private-dialog"), "完成后立即清理 dialog 原文");
        Check(!new RtpPortStopStore(path).CanReuse(60000, DateTime.UtcNow), "正常冷却也能跨存储重建保留");
        Check(store.PruneExpired(DateTime.UtcNow.AddMinutes(2)) == 1 && store.Pending().Count == 0, "到期删除记录");
        File.WriteAllText(path, "broken");
        var rejected = false;
        try { store.CanReuse(60000, DateTime.UtcNow); } catch (JsonException) { rejected = true; }
        Check(rejected, "隔离文件损坏不能当成空状态放行");
    }

    /// <summary>创建测试设备已确认的独立 SIP dialog，不读取任何真实配置。</summary>
    private static SipChannel Dialog(SipDevice device, int localPort, int remotePort, string callId)
    {
        var raw = $"INVITE sip:channel@127.0.0.1:{remotePort} SIP/2.0\r\nVia: SIP/2.0/UDP 127.0.0.1:{localPort};branch=z9hG4bK{callId}\r\nFrom: <sip:34020000002000000001@127.0.0.1>;tag=from\r\nTo: <sip:channel@127.0.0.1>\r\nCall-ID: {callId}\r\nCSeq: 1 INVITE\r\nContact: <sip:34020000002000000001@127.0.0.1:{localPort}>\r\nMax-Forwards: 70\r\nContent-Length: 0\r\n\r\n";
        var invite = SIPRequest.ParseSIPRequest(raw);
        var response = SIPResponse.GetResponse(invite, SIPResponseStatusCodesEnum.Ok, null);
        response.Header.To.ToTag = "to";
        device.LastSipRequest = invite;
        return new SipChannel { ParentId = "device", DeviceId = "channel", InviteSipRequest = invite, InviteSipResponse = response, PushStatus = PushStatus.PUSHON };
    }

    /// <summary>只在回环地址模拟设备应答：不应答、正确 BYE 应答或错误的 INVITE 应答。</summary>
    private static async Task RespondAsync(UdpClient peer, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            UdpReceiveResult packet;
            try { packet = await peer.ReceiveAsync(ct); } catch (ObjectDisposedException) { return; }
            var request = SIPRequest.ParseSIPRequest(Encoding.UTF8.GetString(packet.Buffer));
            if (_replyMode == 0) continue;
            var response = SIPResponse.GetResponse(request, SIPResponseStatusCodesEnum.Ok, null);
            if (_replyMode == 2) response.Header.CSeqMethod = SIPMethodsEnum.INVITE;
            if (_replyMode == 3) response.Header.CSeq--;
            var data = Encoding.UTF8.GetBytes(response.ToString());
            await peer.SendAsync(data, data.Length, packet.RemoteEndPoint);
        }
    }

    /// <summary>断言真实结果并立即报告失败，不吞异常。</summary>
    private static void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        _checks++;
        Console.WriteLine("PASS: " + name);
    }
}
