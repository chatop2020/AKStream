using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew
{
    private ZLMediaKitConfigNew_API? _api;
    private ZLMediaKitConfigNew_Cluster? _cluster;
    private ZLMediaKitConfigNew_General? _general;
    private ZLMediaKitConfigNew_Hook? _hook;
    private ZLMediaKitConfigNew_Http? _http;
    private ZLMediaKitConfigNew_Multicast? _multicast;
    private ZLMediaKitConfigNew_Protocol? _protocol;
    private ZLMediaKitConfigNew_Record? _record;
    private ZLMediaKitConfigNew_Rtp_Proxy? _rtp_proxy;
    private ZLMediaKitConfigNew_HLS? _hls;
    private ZLMediaKitConfigNew_RTC? _rtc;
    private ZLMediaKitConfigNew_RTP? _rtp;
    private ZLMediaKitConfigNew_SRT? _srt;
    private ZLMediaKitConfigNew_RTMP? _rtmp;
    private ZLMediaKitConfigNew_RTSP? _rtsp;
    private ZLMediaKitConfigNew_FFMPEG? _ffmpeg;
    private ZLMediaKitConfigNew_Shell? _shell;


    public ZLMediaKitConfigNew_API Api
    {
        get => _api;
        set => _api = value;
    }

    public ZLMediaKitConfigNew_Cluster Cluster
    {
        get => _cluster;
        set => _cluster = value;
    }

    public ZLMediaKitConfigNew_General General
    {
        get => _general;
        set => _general = value;
    }

    public ZLMediaKitConfigNew_Hook Hook
    {
        get => _hook;
        set => _hook = value;
    }

    public ZLMediaKitConfigNew_Http Http
    {
        get => _http;
        set => _http = value;
    }

    public ZLMediaKitConfigNew_Multicast Multicast
    {
        get => _multicast;
        set => _multicast = value;
    }

    public ZLMediaKitConfigNew_Protocol Protocol
    {
        get => _protocol;
        set => _protocol = value;
    }

    public ZLMediaKitConfigNew_Record Record
    {
        get => _record;
        set => _record = value;
    }

    public ZLMediaKitConfigNew_Rtp_Proxy Rtp_Proxy
    {
        get => _rtp_proxy;
        set => _rtp_proxy = value;
    }

    public ZLMediaKitConfigNew_HLS Hls
    {
        get => _hls;
        set => _hls = value;
    }

    public ZLMediaKitConfigNew_RTC Rtc
    {
        get => _rtc;
        set => _rtc = value;
    }

    public ZLMediaKitConfigNew_RTP Rtp
    {
        get => _rtp;
        set => _rtp = value;
    }

    public ZLMediaKitConfigNew_SRT Srt
    {
        get => _srt;
        set => _srt = value;
    }

    public ZLMediaKitConfigNew_RTMP Rtmp
    {
        get => _rtmp;
        set => _rtmp = value;
    }

    public ZLMediaKitConfigNew_RTSP Rtsp
    {
        get => _rtsp;
        set => _rtsp = value;
    }

    public ZLMediaKitConfigNew_FFMPEG FFmpeg
    {
        get => _ffmpeg;
        set => _ffmpeg = value;
    }

    public ZLMediaKitConfigNew_Shell Shell
    {
        get => _shell;
        set => _shell = value;
    }


    /// <summary>
    /// 写ini文件
    /// </summary>
    /// <param name="configPath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public bool SetConfig(string configPath)
    {
        try
        {
            var parser = new FileIniDataParser();
            IniData data = new IniData();

            #region api部分

            if (Api != null)
            {
                if (Api.ApiDebug != null)
                {
                    data["api"]["apiDebug"] = Api.ApiDebug.ToString();
                }

                if (!string.IsNullOrEmpty(Api.Secret))
                {
                    data["api"]["secret"] = Api.Secret.Trim();
                }

                if (!string.IsNullOrEmpty(Api.SnapRoot))
                {
                    data["api"]["snapRoot"] = Api.SnapRoot.Trim();
                }

                if (!string.IsNullOrEmpty(Api.SnapRoot))
                {
                    data["api"]["snapRoot"] = Api.SnapRoot.Trim();
                }
            }

            #endregion

            #region ffmpeg部分

            if (FFmpeg != null)
            {
                if (!string.IsNullOrEmpty(FFmpeg.Bin))
                {
                    data["ffmpeg"]["bin"] = FFmpeg.Bin.Trim();
                }

                if (!string.IsNullOrEmpty(FFmpeg.Cmd))
                {
                    data["ffmpeg"]["cmd"] = FFmpeg.Cmd.Trim();
                }

                if (!string.IsNullOrEmpty(FFmpeg.Snap))
                {
                    data["ffmpeg"]["snap"] = FFmpeg.Snap.Trim();
                }

                if (!string.IsNullOrEmpty(FFmpeg.Log))
                {
                    data["ffmpeg"]["log"] = FFmpeg.Log.Trim();
                }

                if (FFmpeg.Restart_Sec != null && UtilsHelper.IsInteger(FFmpeg.Restart_Sec.ToString()))
                {
                    data["ffmpeg"]["restart_sec"] = FFmpeg.Restart_Sec.ToString();
                }
            }

            #endregion

            #region protocol部分

            if (Protocol != null)
            {
                if (Protocol.Modify_Stamp != null && UtilsHelper.IsInteger(Protocol.Modify_Stamp.ToString()))
                {
                    data["protocol"]["modify_stamp"] = Protocol.Modify_Stamp.ToString();
                }

                if (Protocol.Enable_Audio != null && UtilsHelper.IsInteger(Protocol.Enable_Audio.ToString()))
                {
                    data["protocol"]["enable_audio"] = Protocol.Enable_Audio.ToString();
                }

                if (Protocol.Add_Mute_Audio != null && UtilsHelper.IsInteger(Protocol.Add_Mute_Audio.ToString()))
                {
                    data["protocol"]["add_mute_audio"] = Protocol.Add_Mute_Audio.ToString();
                }

                if (Protocol.Auto_Close != null && UtilsHelper.IsInteger(Protocol.Auto_Close.ToString()))
                {
                    data["protocol"]["auto_close"] = Protocol.Auto_Close.ToString();
                }

                if (Protocol.Continue_Push_Ms != null && UtilsHelper.IsInteger(Protocol.Continue_Push_Ms.ToString()))
                {
                    data["protocol"]["continue_push_ms"] = Protocol.Continue_Push_Ms.ToString();
                }

                if (Protocol.Enable_Hls != null && UtilsHelper.IsInteger(Protocol.Enable_Hls.ToString()))
                {
                    data["protocol"]["enable_hls"] = Protocol.Enable_Hls.ToString();
                }

                if (Protocol.Enable_Hls_Fmp4 != null && UtilsHelper.IsInteger(Protocol.Enable_Hls_Fmp4.ToString()))
                {
                    data["protocol"]["enable_hls_fmp4"] = Protocol.Enable_Hls_Fmp4.ToString();
                }

                if (Protocol.Enable_Mp4 != null && UtilsHelper.IsInteger(Protocol.Enable_Mp4.ToString()))
                {
                    data["protocol"]["enable_mp4"] = Protocol.Enable_Mp4.ToString();
                }

                if (Protocol.Enable_Rtsp != null && UtilsHelper.IsInteger(Protocol.Enable_Rtsp.ToString()))
                {
                    data["protocol"]["enable_rtsp"] = Protocol.Enable_Rtsp.ToString();
                }

                if (Protocol.Enable_Rtmp != null && UtilsHelper.IsInteger(Protocol.Enable_Rtmp.ToString()))
                {
                    data["protocol"]["enable_rtmp"] = Protocol.Enable_Rtmp.ToString();
                }

                if (Protocol.Enable_Ts != null && UtilsHelper.IsInteger(Protocol.Enable_Ts.ToString()))
                {
                    data["protocol"]["enable_ts"] = Protocol.Enable_Ts.ToString();
                }

                if (Protocol.Enable_Fmp4 != null && UtilsHelper.IsInteger(Protocol.Enable_Fmp4.ToString()))
                {
                    data["protocol"]["enable_fmp4"] = Protocol.Enable_Fmp4.ToString();
                }

                if (Protocol.Mp4_As_Player != null && UtilsHelper.IsInteger(Protocol.Mp4_As_Player.ToString()))
                {
                    data["protocol"]["mp4_as_player"] = Protocol.Mp4_As_Player.ToString();
                }

                if (Protocol.Mp4_Max_Second != null && UtilsHelper.IsInteger(Protocol.Mp4_Max_Second.ToString()))
                {
                    data["protocol"]["mp4_max_second"] = Protocol.Mp4_Max_Second.ToString();
                }

                if (Protocol.Mp4_Save_Path != null && !string.IsNullOrEmpty(Protocol.Mp4_Save_Path))
                {
                    data["protocol"]["mp4_save_path"] = Protocol.Mp4_Save_Path.Trim();
                }

                if (Protocol.Hls_Save_Path != null && !string.IsNullOrEmpty(Protocol.Hls_Save_Path))
                {
                    data["protocol"]["hls_save_path"] = Protocol.Hls_Save_Path.Trim();
                }

                if (Protocol.Hls_Demand != null && UtilsHelper.IsInteger(Protocol.Hls_Demand.ToString()))
                {
                    data["protocol"]["hls_demand"] = Protocol.Hls_Demand.ToString();
                }

                if (Protocol.Rtsp_Demand != null && UtilsHelper.IsInteger(Protocol.Rtsp_Demand.ToString()))
                {
                    data["protocol"]["rtsp_demand"] = Protocol.Rtsp_Demand.ToString();
                }

                if (Protocol.Ts_Demand != null && UtilsHelper.IsInteger(Protocol.Ts_Demand.ToString()))
                {
                    data["protocol"]["ts_demand"] = Protocol.Ts_Demand.ToString();
                }

                if (Protocol.Fmp4_Demand != null && UtilsHelper.IsInteger(Protocol.Fmp4_Demand.ToString()))
                {
                    data["protocol"]["fmp4_demand"] = Protocol.Fmp4_Demand.ToString();
                }
            }

            #endregion

            #region general部分

            if (General != null)
            {
                if (General.EnableVhost != null && UtilsHelper.IsInteger(General.EnableVhost.ToString()))
                {
                    data["general"]["enableVhost"] = General.EnableVhost.ToString();
                }

                if (General.FlowThreshold != null && UtilsHelper.IsInteger(General.FlowThreshold.ToString()))
                {
                    data["general"]["flowThreshold"] = General.FlowThreshold.ToString();
                }

                if (General.MaxStreamWaitMs != null && UtilsHelper.IsInteger(General.MaxStreamWaitMs.ToString()))
                {
                    data["general"]["maxStreamWaitMS"] = General.MaxStreamWaitMs.ToString();
                }

                if (General.StreamNoneReaderDelayMs != null &&
                    UtilsHelper.IsInteger(General.StreamNoneReaderDelayMs.ToString()))
                {
                    data["general"]["streamNoneReaderDelayMS"] = General.StreamNoneReaderDelayMs.ToString();
                }

                if (General.ResetWhenRePlay != null && UtilsHelper.IsInteger(General.ResetWhenRePlay.ToString()))
                {
                    data["general"]["resetWhenRePlay"] = General.ResetWhenRePlay.ToString();
                }

                if (General.MergeWriteMs != null && UtilsHelper.IsInteger(General.MergeWriteMs.ToString()))
                {
                    data["general"]["mergeWriteMS"] = General.MergeWriteMs.ToString();
                }

                if (General.MediaServerId != null && !string.IsNullOrEmpty(General.MediaServerId))
                {
                    data["general"]["mediaServerId"] = General.MediaServerId.Trim();
                }

                if (General.Wait_Track_Ready_Ms != null &&
                    UtilsHelper.IsInteger(General.Wait_Track_Ready_Ms.ToString()))
                {
                    data["general"]["wait_track_ready_ms"] = General.Wait_Track_Ready_Ms.ToString();
                }

                if (General.Wait_Add_Track_Ms != null && UtilsHelper.IsInteger(General.Wait_Add_Track_Ms.ToString()))
                {
                    data["general"]["wait_add_track_ms"] = General.Wait_Add_Track_Ms.ToString();
                }

                if (General.Unready_Frame_Cache != null &&
                    UtilsHelper.IsInteger(General.Unready_Frame_Cache.ToString()))
                {
                    data["general"]["unready_frame_cache"] = General.Unready_Frame_Cache.ToString();
                }
            }

            #endregion

            #region hls部分

            if (Hls != null)
            {
                if (Hls.FileBufSize != null && UtilsHelper.IsInteger(Hls.FileBufSize.ToString()))
                {
                    data["hls"]["fileBufSize"] = Hls.FileBufSize.ToString();
                }

                if (Hls.SegDur != null && UtilsHelper.IsInteger(Hls.SegDur.ToString()))
                {
                    data["hls"]["segDur"] = Hls.SegDur.ToString();
                }

                if (Hls.SegNum != null && UtilsHelper.IsInteger(Hls.SegNum.ToString()))
                {
                    data["hls"]["segNum"] = Hls.SegNum.ToString();
                }

                if (Hls.SegRetain != null && UtilsHelper.IsInteger(Hls.SegRetain.ToString()))
                {
                    data["hls"]["segRetain"] = Hls.SegRetain.ToString();
                }

                if (Hls.BroadcastRecordTs != null && UtilsHelper.IsInteger(Hls.BroadcastRecordTs.ToString()))
                {
                    data["hls"]["broadcastRecordTs"] = Hls.BroadcastRecordTs.ToString();
                }

                if (Hls.DeleteDelaySec != null && UtilsHelper.IsInteger(Hls.DeleteDelaySec.ToString()))
                {
                    data["hls"]["deleteDelaySec"] = Hls.DeleteDelaySec.ToString();
                }

                if (Hls.SegKeep != null && UtilsHelper.IsInteger(Hls.SegKeep.ToString()))
                {
                    data["hls"]["segKeep"] = Hls.SegKeep.ToString();
                }
            }

            #endregion

            #region hook部分

            if (Hook != null)
            {
                if (!string.IsNullOrEmpty(Hook.On_Server_Exited))
                {
                    data["hook"]["on_server_exited"] = Hook.On_Server_Exited.Trim();
                }

                if (Hook.Enable != null && UtilsHelper.IsInteger(Hook.Enable.ToString()))
                {
                    data["hook"]["enable"] = Hook.Enable.ToString();
                }

                if (!string.IsNullOrEmpty(Hook.On_Flow_Report))
                {
                    data["hook"]["on_flow_report"] = Hook.On_Flow_Report.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Http_Access))
                {
                    data["hook"]["on_http_access"] = Hook.On_Http_Access.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Play))
                {
                    data["hook"]["on_play"] = Hook.On_Play.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Publish))
                {
                    data["hook"]["on_publish"] = Hook.On_Publish.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Record_Mp4))
                {
                    data["hook"]["on_record_mp4"] = Hook.On_Record_Mp4.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Record_Ts))
                {
                    data["hook"]["on_record_ts"] = Hook.On_Record_Ts.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Rtsp_Auth))
                {
                    data["hook"]["on_rtsp_auth"] = Hook.On_Rtsp_Auth.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Rtsp_Realm))
                {
                    data["hook"]["on_rtsp_realm"] = Hook.On_Rtsp_Realm.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Shell_Login))
                {
                    data["hook"]["on_shell_login"] = Hook.On_Shell_Login.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Stream_Changed))
                {
                    data["hook"]["on_stream_changed"] = Hook.On_Stream_Changed.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Stream_None_Reader))
                {
                    data["hook"]["on_stream_none_reader"] = Hook.On_Stream_None_Reader.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Stream_Not_Found))
                {
                    data["hook"]["on_stream_not_found"] = Hook.On_Stream_Not_Found.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Server_Started))
                {
                    data["hook"]["on_server_started"] = Hook.On_Server_Started.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Server_Keepalive))
                {
                    data["hook"]["on_server_keepalive"] = Hook.On_Server_Keepalive.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Send_Rtp_Stopped))
                {
                    data["hook"]["on_send_rtp_stopped"] = Hook.On_Send_Rtp_Stopped.Trim();
                }

                if (!string.IsNullOrEmpty(Hook.On_Rtp_Server_Timeout))
                {
                    data["hook"]["on_rtp_server_timeout"] = Hook.On_Rtp_Server_Timeout.Trim();
                }

                if (Hook.TimeoutSec != null && UtilsHelper.IsInteger(Hook.TimeoutSec.ToString()))
                {
                    data["hook"]["timeoutSec"] = Hook.TimeoutSec.ToString();
                }

                if (Hook.Alive_Interval != null && UtilsHelper.IsFloat(Hook.Alive_Interval.ToString()))
                {
                    data["hook"]["alive_interval"] = Hook.Alive_Interval.ToString();
                }

                if (Hook.Retry != null && UtilsHelper.IsInteger(Hook.Retry.ToString()))
                {
                    data["hook"]["retry"] = Hook.Retry.ToString();
                }

                if (Hook.Retry_Delay != null && UtilsHelper.IsFloat(Hook.Retry_Delay.ToString()))
                {
                    data["hook"]["retry_delay"] = Hook.Retry_Delay.ToString();
                }
            }

            #endregion

            #region cluster部分

            if (Cluster != null)
            {
                if (!string.IsNullOrEmpty(Cluster.Origin_Url))
                {
                    data["cluster"]["origin_url"] = Cluster.Origin_Url.Trim();
                }

                if (Cluster.Timeout_Sec != null && UtilsHelper.IsInteger(Cluster.Timeout_Sec.ToString()))
                {
                    data["cluster"]["timeout_sec"] = Cluster.Timeout_Sec.ToString();
                }

                if (Cluster.Retry_Count != null && UtilsHelper.IsInteger(Cluster.Retry_Count.ToString()))
                {
                    data["cluster"]["retry_count"] = Cluster.Retry_Count.ToString();
                }
            }

            #endregion

            #region http部分

            if (Http != null)
            {
                if (!string.IsNullOrEmpty(Http.CharSet))
                {
                    data["http"]["charSet"] = Http.CharSet.Trim();
                }

                if (Http.KeepAliveSecond != null && UtilsHelper.IsInteger(Http.KeepAliveSecond.ToString()))
                {
                    data["http"]["keepAliveSecond"] = Http.KeepAliveSecond.ToString();
                }

                if (Http.MaxReqSize != null && UtilsHelper.IsInteger(Http.MaxReqSize.ToString()))
                {
                    data["http"]["maxReqSize"] = Http.MaxReqSize.ToString();
                }

                if (!string.IsNullOrEmpty(Http.NotFound))
                {
                    data["http"]["notFound"] = Http.NotFound.Trim();
                }

                if (Http.Port != null && UtilsHelper.IsUShort(Http.Port.ToString()))
                {
                    data["http"]["port"] = Http.Port.ToString();
                }

                if (!string.IsNullOrEmpty(Http.RootPath))
                {
                    data["http"]["rootPath"] = Http.RootPath.Trim();
                }

                if (Http.SendBufSize != null && UtilsHelper.IsInteger(Http.SendBufSize.ToString()))
                {
                    data["http"]["sendBufSize"] = Http.SendBufSize.ToString();
                }

                if (Http.SSLport != null && UtilsHelper.IsUShort(Http.SSLport.ToString()))
                {
                    data["http"]["sslport"] = Http.SSLport.ToString();
                }

                if (Http.DirMenu != null && UtilsHelper.IsInteger(Http.DirMenu.ToString()))
                {
                    data["http"]["dirMenu"] = Http.DirMenu.ToString();
                }

                if (!string.IsNullOrEmpty(Http.VirtualPath))
                {
                    data["http"]["virtualPath"] = Http.VirtualPath.Trim();
                }

                if (!string.IsNullOrEmpty(Http.ForbidCacheSuffix))
                {
                    data["http"]["forbidCacheSuffix"] = Http.ForbidCacheSuffix.Trim();
                }

                if (!string.IsNullOrEmpty(Http.Forwarded_Ip_Header))
                {
                    data["http"]["forwarded_ip_header"] = Http.Forwarded_Ip_Header.Trim();
                }

                if (!string.IsNullOrEmpty(Http.Allow_Cross_Domains))
                {
                    data["http"]["allow_cross_domains"] = Http.Allow_Cross_Domains.Trim();
                }

                data["http"]["allow_ip_range"] = Http.Allow_Ip_Range.Trim() + " "; //为了防止ip无法访问
            }

            #endregion

            #region multicast部分

            if (Multicast != null)
            {
                if (!string.IsNullOrEmpty(Multicast.AddrMax))
                {
                    data["multicast"]["addrMax"] = Multicast.AddrMax.Trim();
                }

                if (!string.IsNullOrEmpty(Multicast.AddrMin))
                {
                    data["multicast"]["addrMin"] = Multicast.AddrMin.Trim();
                }

                if (Multicast.UdpTtl != null && UtilsHelper.IsInteger(Multicast.UdpTtl.ToString()))
                {
                    data["multicast"]["udpTTL"] = Multicast.UdpTtl.ToString();
                }
            }

            #endregion

            #region record部分

            if (Record != null)
            {
                if (!string.IsNullOrEmpty(Record.AppName))
                {
                    data["record"]["appName"] = Record.AppName.Trim();
                }

                if (Record.FileBufSize != null && UtilsHelper.IsInteger(Record.FileBufSize.ToString()))
                {
                    data["record"]["fileBufSize"] = Record.FileBufSize.ToString();
                }

                if (Record.SampleMs != null && UtilsHelper.IsInteger(Record.SampleMs.ToString()))
                {
                    data["record"]["sampleMS"] = Record.SampleMs.ToString();
                }

                if (Record.FastStart != null && UtilsHelper.IsInteger(Record.FastStart.ToString()))
                {
                    data["record"]["fastStart"] = Record.FastStart.ToString();
                }

                if (Record.FileRepeat != null && UtilsHelper.IsInteger(Record.FileRepeat.ToString()))
                {
                    data["record"]["fileRepeat"] = Record.FileRepeat.ToString();
                }
            }

            #endregion

            #region Rtmp部分

            if (Rtmp != null)
            {
                if (Rtmp.HandshakeSecond != null && UtilsHelper.IsInteger(Rtmp.HandshakeSecond.ToString()))
                {
                    data["rtmp"]["handshakeSecond"] = Rtmp.HandshakeSecond.ToString();
                }

                if (Rtmp.KeepAliveSecond != null && UtilsHelper.IsInteger(Rtmp.KeepAliveSecond.ToString()))
                {
                    data["rtmp"]["keepAliveSecond"] = Rtmp.KeepAliveSecond.ToString();
                }

                if (Rtmp.Port != null && UtilsHelper.IsUShort(Rtmp.Port.ToString()))
                {
                    data["rtmp"]["port"] = Rtmp.Port.ToString();
                }

                if (Rtmp.Sslport != null && UtilsHelper.IsUShort(Rtmp.Sslport.ToString()))
                {
                    data["rtmp"]["sslport"] = Rtmp.Sslport.ToString();
                }
            }

            #endregion

            #region rtp部分

            if (Rtp != null)
            {
                if (Rtp.AudioMtuSize != null && UtilsHelper.IsInteger(Rtp.AudioMtuSize.ToString()))
                {
                    data["rtp"]["audioMtuSize"] = Rtp.AudioMtuSize.ToString();
                }

                if (Rtp.VideoMtuSize != null && UtilsHelper.IsInteger(Rtp.VideoMtuSize.ToString()))
                {
                    data["rtp"]["videoMtuSize"] = Rtp.VideoMtuSize.ToString();
                }

                if (Rtp.RtpMaxSize != null && UtilsHelper.IsInteger(Rtp.RtpMaxSize.ToString()))
                {
                    data["rtp"]["rtpMaxSize"] = Rtp.RtpMaxSize.ToString();
                }

                if (Rtp.LowLatency != null && UtilsHelper.IsInteger(Rtp.LowLatency.ToString()))
                {
                    data["rtp"]["lowLatency"] = Rtp.LowLatency.ToString();
                }

                if (Rtp.H264_Stap_A != null && UtilsHelper.IsInteger(Rtp.H264_Stap_A.ToString()))
                {
                    data["rtp"]["h264_stap_a"] = Rtp.H264_Stap_A.ToString();
                }
            }

            #endregion

            #region rtp_proxy部分

            if (Rtp_Proxy != null)
            {
                if (!string.IsNullOrEmpty(Rtp_Proxy.DumpDir))
                {
                    data["rtp_proxy"]["dumpDir"] = Rtp_Proxy.DumpDir.Trim();
                }

                if (Rtp_Proxy.Port != null && UtilsHelper.IsUShort(Rtp_Proxy.Port.ToString()))
                {
                    data["rtp_proxy"]["port"] = Rtp_Proxy.Port.ToString();
                }

                if (Rtp_Proxy.TimeoutSec != null && UtilsHelper.IsInteger(Rtp_Proxy.TimeoutSec.ToString()))
                {
                    data["rtp_proxy"]["timeoutSec"] = Rtp_Proxy.TimeoutSec.ToString();
                }

                if (!string.IsNullOrEmpty(Rtp_Proxy.Port_Range))
                {
                    data["rtp_proxy"]["port_range"] = Rtp_Proxy.Port_Range.Trim();
                }

                if (!string.IsNullOrEmpty(Rtp_Proxy.H264_Pt))
                {
                    data["rtp_proxy"]["h264_pt"] = Rtp_Proxy.H264_Pt.Trim();
                }

                if (!string.IsNullOrEmpty(Rtp_Proxy.H265_Pt))
                {
                    data["rtp_proxy"]["h265_pt"] = Rtp_Proxy.H265_Pt.Trim();
                }

                if (!string.IsNullOrEmpty(Rtp_Proxy.Ps_Pt))
                {
                    data["rtp_proxy"]["ps_pt"] = Rtp_Proxy.Ps_Pt.Trim();
                }

                if (!string.IsNullOrEmpty(Rtp_Proxy.Opus_Pt))
                {
                    data["rtp_proxy"]["opus_pt"] = Rtp_Proxy.Opus_Pt.Trim();
                }

                if (Rtp_Proxy.Gop_Cache != null && UtilsHelper.IsUShort(Rtp_Proxy.Gop_Cache.ToString()))
                {
                    data["rtp_proxy"]["gop_cache"] = Rtp_Proxy.Gop_Cache.ToString();
                }
            }

            #endregion

            #region rtc部分

            if (Rtc != null)
            {
                if (Rtc.TimeoutSec != null && UtilsHelper.IsInteger(Rtc.TimeoutSec.ToString()))
                {
                    data["rtc"]["timeoutSec"] = Rtc.TimeoutSec.ToString();
                }

                if (!string.IsNullOrEmpty(Rtc.ExternIp))
                {
                    data["rtc"]["externIP"] = Rtc.ExternIp.Trim();
                }

                if (Rtc.Port != null && UtilsHelper.IsUShort(Rtc.Port.ToString()))
                {
                    data["rtc"]["port"] = Rtc.Port.ToString();
                }

                if (Rtc.TcpPort != null && UtilsHelper.IsUShort(Rtc.TcpPort.ToString()))
                {
                    data["rtc"]["tcpPort"] = Rtc.TcpPort.ToString();
                }

                if (Rtc.RembBitRate != null && UtilsHelper.IsInteger(Rtc.RembBitRate.ToString()))
                {
                    data["rtc"]["rembBitRate"] = Rtc.RembBitRate.ToString();
                }

                if (!string.IsNullOrEmpty(Rtc.PreferredCodecA))
                {
                    data["rtc"]["preferredCodecA"] = Rtc.PreferredCodecA.Trim();
                }

                if (!string.IsNullOrEmpty(Rtc.PreferredCodecV))
                {
                    data["rtc"]["preferredCodecV"] = Rtc.PreferredCodecV.Trim();
                }
            }

            #endregion

            #region srt部分

            if (Srt != null)
            {
                if (Srt.TimeoutSec != null && UtilsHelper.IsInteger(Srt.TimeoutSec.ToString()))
                {
                    data["srt"]["timeoutSec"] = Srt.TimeoutSec.ToString();
                }

                if (Srt.Port != null && UtilsHelper.IsUShort(Srt.Port.ToString()))
                {
                    data["srt"]["port"] = Srt.Port.ToString();
                }

                if (Srt.LatencyMul != null && UtilsHelper.IsInteger(Srt.LatencyMul.ToString()))
                {
                    data["srt"]["latencyMul"] = Srt.LatencyMul.ToString();
                }

                if (Srt.PktBufSize != null && UtilsHelper.IsInteger(Srt.PktBufSize.ToString()))
                {
                    data["srt"]["pktBufSize"] = Srt.PktBufSize.ToString();
                }
            }

            #endregion

            #region rtsp部分

            if (Rtsp != null)
            {
                if (Rtsp.AuthBasic != null && UtilsHelper.IsInteger(Rtsp.AuthBasic.ToString()))
                {
                    data["rtsp"]["authBasic"] = Rtsp.AuthBasic.ToString();
                }

                if (Rtsp.DirectProxy != null && UtilsHelper.IsInteger(Rtsp.DirectProxy.ToString()))
                {
                    data["rtsp"]["directProxy"] = Rtsp.DirectProxy.ToString();
                }

                if (Rtsp.HandshakeSecond != null && UtilsHelper.IsInteger(Rtsp.HandshakeSecond.ToString()))
                {
                    data["rtsp"]["handshakeSecond"] = Rtsp.HandshakeSecond.ToString();
                }

                if (Rtsp.KeepAliveSecond != null && UtilsHelper.IsInteger(Rtsp.KeepAliveSecond.ToString()))
                {
                    data["rtsp"]["keepAliveSecond"] = Rtsp.KeepAliveSecond.ToString();
                }

                if (Rtsp.Port != null && UtilsHelper.IsUShort(Rtsp.Port.ToString()))
                {
                    data["rtsp"]["port"] = Rtsp.Port.ToString();
                }

                if (Rtsp.Sslport != null && UtilsHelper.IsUShort(Rtsp.Sslport.ToString()))
                {
                    data["rtsp"]["sslport"] = Rtsp.Sslport.ToString();
                }

                if (Rtsp.LowLatency != null && UtilsHelper.IsInteger(Rtsp.LowLatency.ToString()))
                {
                    data["rtsp"]["lowLatency"] = Rtsp.LowLatency.ToString();
                }

                if (Rtsp.RtpTransportType != null && UtilsHelper.IsInteger(Rtsp.RtpTransportType.ToString()))
                {
                    data["rtsp"]["rtpTransportType"] = Rtsp.RtpTransportType.ToString();
                }
            }

            #endregion

            #region shell部分

            if (Shell.MaxReqSize != null && UtilsHelper.IsInteger(Shell.MaxReqSize.ToString()))
            {
                data["shell"]["maxReqSize"] = Shell.MaxReqSize.ToString();
            }

            if (Shell.Port != null && UtilsHelper.IsUShort(Shell.Port.ToString()))
            {
                data["shell"]["port"] = Shell.Port.ToString();
            }

            #endregion

            parser.WriteFile(configPath, data, Encoding.UTF8);
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 从配置文件中获取配置类实例
    /// </summary>
    /// <param name="configPath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public ZLMediaKitConfigNew GetConfigByFilePath(string configPath)
    {
        try
        {
            ZLMediaKitConfigNew result = new ZLMediaKitConfigNew(configPath);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 初始化，读取ini文件
    /// </summary>
    /// <param name="configPath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public ZLMediaKitConfigNew(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(configPath + "文件不存在");
        }

        try
        {
            var parser = new FileIniDataParser();
            string[] fileIniStrings = File.ReadAllLines(configPath);
            for (int i = 0; i <= fileIniStrings.Length - 1; i++)
            {
                if (fileIniStrings[i].Trim().StartsWith('#') || fileIniStrings[i].Trim().StartsWith(';'))
                {
                    fileIniStrings[i] = fileIniStrings[i].TrimStart('#');
                    fileIniStrings[i] = ";" + fileIniStrings[i];
                }
            }

            File.WriteAllLines(configPath, fileIniStrings);
            IniData data = parser.ReadFile(configPath, Encoding.UTF8);

            #region api部分

            if (Api == null)
            {
                Api = new ZLMediaKitConfigNew_API();
            }

            var apiDebug = data["api"]["apiDebug"];
            if (apiDebug != null && !string.IsNullOrEmpty(apiDebug) && UtilsHelper.IsInteger(apiDebug))
            {
                Api.ApiDebug = int.Parse(apiDebug);
            }

            var secret = data["api"]["secret"];
            if (secret != null && !string.IsNullOrEmpty(secret))
            {
                Api.Secret = secret;
            }

            var snapRoot = data["api"]["snapRoot"];
            if (snapRoot != null && !string.IsNullOrEmpty(snapRoot))
            {
                Api.SnapRoot = snapRoot;
            }

            var defaultSnap = data["api"]["defaultSnap"];
            if (defaultSnap != null && !string.IsNullOrEmpty(defaultSnap))
            {
                Api.DefaultSnap = defaultSnap;
            }

            #endregion

            #region ffmpeg部分

            if (FFmpeg == null)
            {
                FFmpeg = new ZLMediaKitConfigNew_FFMPEG();
            }

            var bin = data["ffmpeg"]["bin"];
            if (bin != null && !string.IsNullOrEmpty(bin))
            {
                FFmpeg.Bin = bin;
            }

            var cmd = data["ffmpeg"]["cmd"];
            if (cmd != null && !string.IsNullOrEmpty(cmd))
            {
                FFmpeg.Cmd = cmd;
            }

            var snap = data["ffmpeg"]["snap"];
            if (snap != null && !string.IsNullOrEmpty(snap))
            {
                FFmpeg.Snap = snap;
            }

            var log = data["ffmpeg"]["log"];
            if (log != null && !string.IsNullOrEmpty(log))
            {
                FFmpeg.Log = log;
            }

            var restart_sec = data["ffmpeg"]["restart_sec"];
            if (restart_sec != null && !string.IsNullOrEmpty(restart_sec) && UtilsHelper.IsInteger(restart_sec))
            {
                FFmpeg.Restart_Sec = int.Parse(restart_sec);
            }

            #endregion

            #region protocol部分

            if (Protocol == null)
            {
                Protocol = new ZLMediaKitConfigNew_Protocol();
            }

            var modify_stamp = data["protocol"]["modify_stamp"];
            if (modify_stamp != null && !string.IsNullOrEmpty(modify_stamp) &&
                UtilsHelper.IsInteger(modify_stamp))
            {
                Protocol.Modify_Stamp = int.Parse(modify_stamp);
            }

            var enable_audio = data["protocol"]["enable_audio"];
            if (enable_audio != null && !string.IsNullOrEmpty(enable_audio) &&
                UtilsHelper.IsInteger(enable_audio))
            {
                Protocol.Enable_Audio = int.Parse(enable_audio);
            }

            var add_mute_audio = data["protocol"]["add_mute_audio"];
            if (add_mute_audio != null && !string.IsNullOrEmpty(add_mute_audio) &&
                UtilsHelper.IsInteger(add_mute_audio))
            {
                Protocol.Add_Mute_Audio = int.Parse(add_mute_audio);
            }

            var auto_close = data["protocol"]["auto_close"];
            if (auto_close != null && !string.IsNullOrEmpty(auto_close) &&
                UtilsHelper.IsInteger(auto_close))
            {
                Protocol.Auto_Close = int.Parse(auto_close);
            }

            var continue_push_ms = data["protocol"]["continue_push_ms"];
            if (continue_push_ms != null && !string.IsNullOrEmpty(continue_push_ms) &&
                UtilsHelper.IsInteger(continue_push_ms))
            {
                Protocol.Continue_Push_Ms = int.Parse(continue_push_ms);
            }

            var enable_hls = data["protocol"]["enable_hls"];
            if (enable_hls != null && !string.IsNullOrEmpty(enable_hls) && UtilsHelper.IsInteger(enable_hls))
            {
                Protocol.Enable_Hls = int.Parse(enable_hls);
            }

            var enable_hls_fmp4 = data["protocol"]["enable_hls_fmp4"];
            if (enable_hls_fmp4 != null && !string.IsNullOrEmpty(enable_hls_fmp4) &&
                UtilsHelper.IsInteger(enable_hls_fmp4))
            {
                Protocol.Enable_Hls_Fmp4 = int.Parse(enable_hls_fmp4);
            }

            var enable_mp4 = data["protocol"]["enable_mp4"];
            if (enable_mp4 != null && !string.IsNullOrEmpty(enable_mp4) && UtilsHelper.IsInteger(enable_mp4))
            {
                Protocol.Enable_Mp4 = int.Parse(enable_mp4);
            }

            var enable_rtsp = data["protocol"]["enable_rtsp"];
            if (enable_rtsp != null && !string.IsNullOrEmpty(enable_rtsp) && UtilsHelper.IsInteger(enable_rtsp))
            {
                Protocol.Enable_Rtsp = int.Parse(enable_rtsp);
            }

            var enable_rtmp = data["protocol"]["enable_rtmp"];
            if (enable_rtmp != null && !string.IsNullOrEmpty(enable_rtmp) && UtilsHelper.IsInteger(enable_rtmp))
            {
                Protocol.Enable_Rtmp = int.Parse(enable_rtmp);
            }

            var enable_ts = data["protocol"]["enable_ts"];
            if (enable_ts != null && !string.IsNullOrEmpty(enable_ts) && UtilsHelper.IsInteger(enable_ts))
            {
                Protocol.Enable_Ts = int.Parse(enable_ts);
            }

            var enable_fmp4 = data["protocol"]["enable_fmp4"];
            if (enable_fmp4 != null && !string.IsNullOrEmpty(enable_fmp4) && UtilsHelper.IsInteger(enable_fmp4))
            {
                Protocol.Enable_Fmp4 = int.Parse(enable_fmp4);
            }

            var mp4_as_player = data["protocol"]["mp4_as_player"];
            if (mp4_as_player != null && !string.IsNullOrEmpty(mp4_as_player) &&
                UtilsHelper.IsInteger(mp4_as_player))
            {
                Protocol.Mp4_As_Player = int.Parse(mp4_as_player);
            }

            var mp4_max_second = data["protocol"]["mp4_max_second"];
            if (mp4_max_second != null && !string.IsNullOrEmpty(mp4_max_second) &&
                UtilsHelper.IsInteger(mp4_max_second))
            {
                Protocol.Mp4_Max_Second = int.Parse(mp4_max_second);
            }

            var mp4_save_path = data["protocol"]["mp4_save_path"];
            if (mp4_save_path != null && !string.IsNullOrEmpty(mp4_save_path))
            {
                Protocol.Mp4_Save_Path = mp4_save_path.Trim();
            }

            var hls_save_path = data["protocol"]["hls_save_path"];
            if (hls_save_path != null && !string.IsNullOrEmpty(hls_save_path))
            {
                Protocol.Hls_Save_Path = hls_save_path.Trim();
            }

            var hls_demand = data["protocol"]["hls_demand"];
            if (hls_demand != null && !string.IsNullOrEmpty(hls_demand) && UtilsHelper.IsInteger(hls_demand))
            {
                Protocol.Hls_Demand = int.Parse(hls_demand);
            }

            var rtsp_demand = data["protocol"]["rtsp_demand"];
            if (rtsp_demand != null && !string.IsNullOrEmpty(rtsp_demand) && UtilsHelper.IsInteger(rtsp_demand))
            {
                Protocol.Rtsp_Demand = int.Parse(rtsp_demand);
            }

            var rtmp_demand = data["protocol"]["rtmp_demand"];
            if (rtmp_demand != null && !string.IsNullOrEmpty(rtmp_demand) && UtilsHelper.IsInteger(rtmp_demand))
            {
                Protocol.Rtmp_Demand = int.Parse(rtmp_demand);
            }

            var ts_demand = data["protocol"]["ts_demand"];
            if (ts_demand != null && !string.IsNullOrEmpty(ts_demand) && UtilsHelper.IsInteger(ts_demand))
            {
                Protocol.Ts_Demand = int.Parse(ts_demand);
            }

            var fmp4_demand = data["protocol"]["fmp4_demand"];
            if (fmp4_demand != null && !string.IsNullOrEmpty(fmp4_demand) && UtilsHelper.IsInteger(fmp4_demand))
            {
                Protocol.Fmp4_Demand = int.Parse(fmp4_demand);
            }

            #endregion

            #region general部分

            if (General == null)
            {
                General = new ZLMediaKitConfigNew_General();
            }

            var enableVhost = data["general"]["enableVhost"];
            if (enableVhost != null && !string.IsNullOrEmpty(enableVhost) && UtilsHelper.IsInteger(enableVhost))
            {
                General.EnableVhost = int.Parse(enableVhost);
            }

            var flowThreshold = data["general"]["flowThreshold"];
            if (flowThreshold != null && !string.IsNullOrEmpty(flowThreshold) &&
                UtilsHelper.IsInteger(flowThreshold))
            {
                General.FlowThreshold = int.Parse(flowThreshold);
            }

            var maxStreamWaitMS = data["general"]["maxStreamWaitMS"];
            if (maxStreamWaitMS != null && !string.IsNullOrEmpty(maxStreamWaitMS) &&
                UtilsHelper.IsInteger(maxStreamWaitMS))
            {
                General.MaxStreamWaitMs = int.Parse(maxStreamWaitMS);
            }

            var streamNoneReaderDelayMS = data["general"]["streamNoneReaderDelayMS"];
            if (streamNoneReaderDelayMS != null && !string.IsNullOrEmpty(streamNoneReaderDelayMS) &&
                UtilsHelper.IsInteger(streamNoneReaderDelayMS))
            {
                General.StreamNoneReaderDelayMs = int.Parse(streamNoneReaderDelayMS);
            }

            var resetWhenRePlay = data["general"]["resetWhenRePlay"];
            if (resetWhenRePlay != null && !string.IsNullOrEmpty(resetWhenRePlay) &&
                UtilsHelper.IsInteger(resetWhenRePlay))
            {
                General.ResetWhenRePlay = int.Parse(resetWhenRePlay);
            }

            var mergeWriteMS = data["general"]["mergeWriteMS"];
            if (mergeWriteMS != null && !string.IsNullOrEmpty(mergeWriteMS) &&
                UtilsHelper.IsInteger(mergeWriteMS))
            {
                General.MergeWriteMs = int.Parse(mergeWriteMS);
            }

            var mediaServerId = data["general"]["mediaServerId"];
            if (mediaServerId != null && !string.IsNullOrEmpty(mediaServerId))
            {
                General.MediaServerId = mediaServerId.Trim();
            }

            var wait_track_ready_ms = data["general"]["wait_track_ready_ms"];
            if (wait_track_ready_ms != null && !string.IsNullOrEmpty(wait_track_ready_ms) &&
                UtilsHelper.IsInteger(wait_track_ready_ms))
            {
                General.Wait_Track_Ready_Ms = int.Parse(wait_track_ready_ms);
            }

            var wait_add_track_ms = data["general"]["wait_add_track_ms"];
            if (wait_add_track_ms != null && !string.IsNullOrEmpty(wait_add_track_ms) &&
                UtilsHelper.IsInteger(wait_add_track_ms))
            {
                General.Wait_Add_Track_Ms = int.Parse(wait_add_track_ms);
            }

            var unready_frame_cache = data["general"]["unready_frame_cache"];
            if (unready_frame_cache != null && !string.IsNullOrEmpty(unready_frame_cache) &&
                UtilsHelper.IsInteger(unready_frame_cache))
            {
                General.Unready_Frame_Cache = int.Parse(unready_frame_cache);
            }

            var check_nvidia_dev = data["general"]["check_nvidia_dev"];
            if (check_nvidia_dev != null && !string.IsNullOrEmpty(check_nvidia_dev) &&
                UtilsHelper.IsInteger(check_nvidia_dev))
            {
                General.Check_Nvidia_Dev = int.Parse(check_nvidia_dev);
            }

            var enable_ffmpeg_log = data["general"]["enable_ffmpeg_log"];
            if (enable_ffmpeg_log != null && !string.IsNullOrEmpty(enable_ffmpeg_log) &&
                UtilsHelper.IsInteger(enable_ffmpeg_log))
            {
                General.Enable_FFmpeg_Log = int.Parse(enable_ffmpeg_log);
            }

            #endregion

            #region hls部分

            if (Hls == null)
            {
                Hls = new ZLMediaKitConfigNew_HLS();
            }

            var fileBufSize = data["hls"]["fileBufSize"];
            if (fileBufSize != null && !string.IsNullOrEmpty(fileBufSize) && UtilsHelper.IsInteger(fileBufSize))
            {
                Hls.FileBufSize = int.Parse(fileBufSize);
            }

            var segDur = data["hls"]["segDur"];
            if (segDur != null && !string.IsNullOrEmpty(segDur) && UtilsHelper.IsInteger(segDur))
            {
                Hls.SegDur = int.Parse(segDur);
            }

            var segNum = data["hls"]["segNum"];
            if (segNum != null && !string.IsNullOrEmpty(segNum) && UtilsHelper.IsInteger(segNum))
            {
                Hls.SegNum = int.Parse(segNum);
            }

            var segRetain = data["hls"]["segRetain"];
            if (segRetain != null && !string.IsNullOrEmpty(segRetain) && UtilsHelper.IsInteger(segRetain))
            {
                Hls.SegRetain = int.Parse(segRetain);
            }

            var broadcastRecordTs = data["hls"]["broadcastRecordTs"];
            if (broadcastRecordTs != null && !string.IsNullOrEmpty(broadcastRecordTs) &&
                UtilsHelper.IsInteger(broadcastRecordTs))
            {
                Hls.BroadcastRecordTs = int.Parse(broadcastRecordTs);
            }

            var deleteDelaySec = data["hls"]["deleteDelaySec"];
            if (deleteDelaySec != null && !string.IsNullOrEmpty(deleteDelaySec) &&
                UtilsHelper.IsInteger(deleteDelaySec))
            {
                Hls.DeleteDelaySec = int.Parse(deleteDelaySec);
            }

            var segKeep = data["hls"]["segKeep"];
            if (segKeep != null && !string.IsNullOrEmpty(segKeep) && UtilsHelper.IsInteger(segKeep))
            {
                Hls.SegKeep = int.Parse(segKeep);
            }

            #endregion

            #region hook部分

            if (Hook == null)
            {
                Hook = new ZLMediaKitConfigNew_Hook();
            }

            var on_server_exited = data["hook"]["on_server_exited"];
            if (on_server_exited != null && !string.IsNullOrEmpty(on_server_exited))
            {
                Hook.On_Server_Exited = on_server_exited.Trim();
            }

            var enable = data["hook"]["enable"];
            if (enable != null && !string.IsNullOrEmpty(enable) && UtilsHelper.IsInteger(enable))
            {
                Hook.Enable = int.Parse(enable);
            }

            var on_flow_report = data["hook"]["on_flow_report"];
            if (on_flow_report != null && !string.IsNullOrEmpty(on_flow_report))
            {
                Hook.On_Flow_Report = on_flow_report.Trim();
            }

            var on_http_access = data["hook"]["on_http_access"];
            if (on_http_access != null && !string.IsNullOrEmpty(on_http_access))
            {
                Hook.On_Http_Access = on_http_access.Trim();
            }

            var on_play = data["hook"]["on_play"];
            if (on_play != null && !string.IsNullOrEmpty(on_play))
            {
                Hook.On_Play = on_play.Trim();
            }

            var on_publish = data["hook"]["on_publish"];
            if (on_publish != null && !string.IsNullOrEmpty(on_publish))
            {
                Hook.On_Publish = on_publish.Trim();
            }

            var on_record_mp4 = data["hook"]["on_record_mp4"];
            if (on_record_mp4 != null && !string.IsNullOrEmpty(on_record_mp4))
            {
                Hook.On_Record_Mp4 = on_record_mp4.Trim();
            }

            var on_record_ts = data["hook"]["on_record_ts"];
            if (on_record_ts != null && !string.IsNullOrEmpty(on_record_ts))
            {
                Hook.On_Record_Ts = on_record_ts.Trim();
            }

            var on_rtsp_auth = data["hook"]["on_rtsp_auth"];
            if (on_rtsp_auth != null && !string.IsNullOrEmpty(on_rtsp_auth))
            {
                Hook.On_Rtsp_Auth = on_rtsp_auth.Trim();
            }

            var on_rtsp_realm = data["hook"]["on_rtsp_realm"];
            if (on_rtsp_realm != null && !string.IsNullOrEmpty(on_rtsp_realm))
            {
                Hook.On_Rtsp_Realm = on_rtsp_realm.Trim();
            }

            var on_shell_login = data["hook"]["on_shell_login"];
            if (on_shell_login != null && !string.IsNullOrEmpty(on_shell_login))
            {
                Hook.On_Shell_Login = on_shell_login.Trim();
            }

            var on_stream_changed = data["hook"]["on_stream_changed"];
            if (on_stream_changed != null && !string.IsNullOrEmpty(on_stream_changed))
            {
                Hook.On_Stream_Changed = on_stream_changed.Trim();
            }

            var on_stream_none_reader = data["hook"]["on_stream_none_reader"];
            if (on_stream_none_reader != null && !string.IsNullOrEmpty(on_stream_none_reader))
            {
                Hook.On_Stream_None_Reader = on_stream_none_reader.Trim();
            }

            var on_stream_not_found = data["hook"]["on_stream_not_found"];
            if (on_stream_not_found != null && !string.IsNullOrEmpty(on_stream_not_found))
            {
                Hook.On_Stream_Not_Found = on_stream_not_found.Trim();
            }

            var on_server_started = data["hook"]["on_server_started"];
            if (on_server_started != null && !string.IsNullOrEmpty(on_server_started))
            {
                Hook.On_Server_Started = on_server_started.Trim();
            }

            var on_server_keepalive = data["hook"]["on_server_keepalive"];
            if (on_server_keepalive != null && !string.IsNullOrEmpty(on_server_keepalive))
            {
                Hook.On_Server_Keepalive = on_server_keepalive.Trim();
            }

            var on_send_rtp_stopped = data["hook"]["on_send_rtp_stopped"];
            if (on_send_rtp_stopped != null && !string.IsNullOrEmpty(on_send_rtp_stopped))
            {
                Hook.On_Send_Rtp_Stopped = on_send_rtp_stopped.Trim();
            }

            var on_rtp_server_timeout = data["hook"]["on_rtp_server_timeout"];
            if (on_rtp_server_timeout != null && !string.IsNullOrEmpty(on_rtp_server_timeout))
            {
                Hook.On_Rtp_Server_Timeout = on_rtp_server_timeout.Trim();
            }

            var timeoutSec = data["hook"]["timeoutSec"];
            if (timeoutSec != null && !string.IsNullOrEmpty(timeoutSec) && UtilsHelper.IsInteger(timeoutSec))
            {
                Hook.TimeoutSec = int.Parse(timeoutSec);
            }

            var alive_interval = data["hook"]["alive_interval"];
            if (alive_interval != null && !string.IsNullOrEmpty(alive_interval) &&
                UtilsHelper.IsFloat(alive_interval))
            {
                Hook.Alive_Interval = float.Parse(alive_interval);
            }

            var retry = data["hook"]["retry"];
            if (retry != null && !string.IsNullOrEmpty(retry) && UtilsHelper.IsInteger(retry))
            {
                Hook.Retry = int.Parse(retry);
            }

            var retry_delay = data["hook"]["retry_delay"];
            if (retry_delay != null && !string.IsNullOrEmpty(retry_delay) && UtilsHelper.IsFloat(retry_delay))
            {
                Hook.Retry_Delay = float.Parse(retry_delay);
            }

            #endregion

            #region cluster部分

            if (Cluster == null)
            {
                Cluster = new ZLMediaKitConfigNew_Cluster();
            }

            var origin_url = data["cluster"]["origin_url"];
            if (origin_url != null && !string.IsNullOrEmpty(origin_url))
            {
                Cluster.Origin_Url = origin_url.Trim();
            }

            var timeout_sec = data["cluster"]["timeout_sec"];
            if (timeout_sec != null && !string.IsNullOrEmpty(timeout_sec) && UtilsHelper.IsInteger(timeout_sec))
            {
                Cluster.Timeout_Sec = int.Parse(timeout_sec);
            }

            var retry_count = data["cluster"]["retry_count"];
            if (retry_count != null && !string.IsNullOrEmpty(retry_count) && UtilsHelper.IsInteger(retry_count))
            {
                Cluster.Retry_Count = int.Parse(retry_count);
            }

            #endregion

            #region http部分

            if (Http == null)
            {
                Http = new ZLMediaKitConfigNew_Http();
            }

            var charSet = data["http"]["charSet"];
            if (charSet != null && !string.IsNullOrEmpty(charSet))
            {
                Http.CharSet = charSet.Trim();
            }

            var keepAliveSecond = data["http"]["keepAliveSecond"];
            if (keepAliveSecond != null && !string.IsNullOrEmpty(keepAliveSecond) &&
                UtilsHelper.IsInteger(keepAliveSecond))
            {
                Http.KeepAliveSecond = int.Parse(keepAliveSecond);
            }

            var maxReqSize = data["http"]["maxReqSize"];
            if (maxReqSize != null && !string.IsNullOrEmpty(maxReqSize) && UtilsHelper.IsInteger(maxReqSize))
            {
                Http.MaxReqSize = int.Parse(maxReqSize);
            }

            var notFound = data["http"]["notFound"];
            if (notFound != null && !string.IsNullOrEmpty(notFound))
            {
                Http.NotFound = notFound.Trim();
            }

            var port = data["http"]["port"];
            if (port != null && !string.IsNullOrEmpty(port) && UtilsHelper.IsUShort(port))
            {
                Http.Port = ushort.Parse(port);
            }

            var rootPath = data["http"]["rootPath"];
            if (rootPath != null && !string.IsNullOrEmpty(rootPath))
            {
                Http.RootPath = rootPath.Trim();
            }

            var sendBufSize = data["http"]["sendBufSize"];
            if (sendBufSize != null && !string.IsNullOrEmpty(sendBufSize) && UtilsHelper.IsInteger(sendBufSize))
            {
                Http.SendBufSize = int.Parse(sendBufSize);
            }

            var sslport = data["http"]["sslport"];
            if (sslport != null && !string.IsNullOrEmpty(sslport) && UtilsHelper.IsUShort(sslport))
            {
                Http.SSLport = ushort.Parse(sslport);
            }

            var dirMenu = data["http"]["dirMenu"];
            if (dirMenu != null && !string.IsNullOrEmpty(dirMenu) && UtilsHelper.IsInteger(dirMenu))
            {
                Http.DirMenu = int.Parse(dirMenu);
            }

            var virtualPath = data["http"]["virtualPath"];
            if (virtualPath != null && !string.IsNullOrEmpty(virtualPath))
            {
                Http.VirtualPath = virtualPath.Trim();
            }

            var forbidCacheSuffix = data["http"]["forbidCacheSuffix"];
            if (forbidCacheSuffix != null && !string.IsNullOrEmpty(forbidCacheSuffix))
            {
                Http.ForbidCacheSuffix = forbidCacheSuffix.Trim();
            }

            var forwarded_ip_header = data["http"]["forwarded_ip_header"];
            if (forwarded_ip_header != null && !string.IsNullOrEmpty(forwarded_ip_header))
            {
                Http.Forwarded_Ip_Header = forwarded_ip_header.Trim();
            }

            var allow_cross_domains = data["http"]["allow_cross_domains"];
            if (allow_cross_domains != null && !string.IsNullOrEmpty(allow_cross_domains))
            {
                Http.Allow_Cross_Domains = allow_cross_domains.Trim();
            }

            var allow_ip_range = data["http"]["allow_ip_range"];
            if (allow_ip_range != null && !string.IsNullOrEmpty(allow_ip_range))
            {
                Http.Allow_Ip_Range = allow_ip_range.Trim();
            }

            #endregion

            #region multicast 部分

            if (Multicast == null)
            {
                Multicast = new ZLMediaKitConfigNew_Multicast();
            }

            var addrMax = data["multicast"]["addrMax"];
            if (addrMax != null && !string.IsNullOrEmpty(addrMax))
            {
                Multicast.AddrMax = addrMax.Trim();
            }

            var addrMin = data["multicast"]["addrMin"];
            if (addrMin != null && !string.IsNullOrEmpty(addrMin))
            {
                Multicast.AddrMin = addrMin.Trim();
            }

            var udpTTL = data["multicast"]["udpTTL"];
            if (udpTTL != null && !string.IsNullOrEmpty(udpTTL) && UtilsHelper.IsInteger(udpTTL))
            {
                Multicast.UdpTtl = int.Parse(udpTTL);
            }

            #endregion

            #region record部分

            if (Record == null)
            {
                Record = new ZLMediaKitConfigNew_Record();
            }

            var appName = data["record"]["appName"];
            if (appName != null && !string.IsNullOrEmpty(appName))
            {
                Record.AppName = appName.Trim();
            }

            var record_fileBufSize = data["record"]["fileBufSize"];
            if (record_fileBufSize != null && !string.IsNullOrEmpty(record_fileBufSize) &&
                UtilsHelper.IsInteger(record_fileBufSize))
            {
                Record.FileBufSize = int.Parse(record_fileBufSize);
            }

            var sampleMS = data["record"]["sampleMS"];
            if (sampleMS != null && !string.IsNullOrEmpty(sampleMS) && UtilsHelper.IsInteger(sampleMS))
            {
                Record.SampleMs = int.Parse(sampleMS);
            }

            var fastStart = data["record"]["fastStart"];
            if (fastStart != null && !string.IsNullOrEmpty(fastStart) && UtilsHelper.IsInteger(fastStart))
            {
                Record.FastStart = int.Parse(fastStart);
            }

            var fileRepeat = data["record"]["fileRepeat"];
            if (fileRepeat != null && !string.IsNullOrEmpty(fileRepeat) && UtilsHelper.IsInteger(fileRepeat))
            {
                Record.FileRepeat = int.Parse(fileRepeat);
            }

            #endregion

            #region rtmp部分

            if (Rtmp == null)
            {
                Rtmp = new ZLMediaKitConfigNew_RTMP();
            }

            var handshakeSecond = data["rtmp"]["handshakeSecond"];
            if (handshakeSecond != null && !string.IsNullOrEmpty(handshakeSecond) &&
                UtilsHelper.IsInteger(handshakeSecond))
            {
                Rtmp.HandshakeSecond = int.Parse(handshakeSecond);
            }

            var rtmp_keepAliveSecond = data["rtmp"]["keepAliveSecond"];
            if (rtmp_keepAliveSecond != null && !string.IsNullOrEmpty(rtmp_keepAliveSecond) &&
                UtilsHelper.IsInteger(rtmp_keepAliveSecond))
            {
                Rtmp.KeepAliveSecond = int.Parse(rtmp_keepAliveSecond);
            }

            var rtmp_port = data["rtmp"]["port"];
            if (rtmp_port != null && !string.IsNullOrEmpty(rtmp_port) && UtilsHelper.IsUShort(rtmp_port))
            {
                Rtmp.Port = ushort.Parse(rtmp_port);
            }

            var rtmp_sslport = data["rtmp"]["sslport"];
            if (rtmp_sslport != null && !string.IsNullOrEmpty(rtmp_sslport) &&
                UtilsHelper.IsUShort(rtmp_sslport))
            {
                Rtmp.Sslport = ushort.Parse(rtmp_sslport);
            }

            #endregion

            #region rtp部分

            if (Rtp == null)
            {
                Rtp = new ZLMediaKitConfigNew_RTP();
            }

            var audioMtuSize = data["rtp"]["audioMtuSize"];
            if (audioMtuSize != null && !string.IsNullOrEmpty(audioMtuSize) &&
                UtilsHelper.IsInteger(audioMtuSize))
            {
                Rtp.AudioMtuSize = int.Parse(audioMtuSize);
            }

            var videoMtuSize = data["rtp"]["videoMtuSize"];
            if (videoMtuSize != null && !string.IsNullOrEmpty(videoMtuSize) &&
                UtilsHelper.IsInteger(videoMtuSize))
            {
                Rtp.VideoMtuSize = int.Parse(videoMtuSize);
            }

            var rtpMaxSize = data["rtp"]["rtpMaxSize"];
            if (rtpMaxSize != null && !string.IsNullOrEmpty(rtpMaxSize) && UtilsHelper.IsInteger(rtpMaxSize))
            {
                Rtp.RtpMaxSize = int.Parse(rtpMaxSize);
            }

            var lowLatency = data["rtp"]["lowLatency"];
            if (lowLatency != null && !string.IsNullOrEmpty(lowLatency) && UtilsHelper.IsInteger(lowLatency))
            {
                Rtp.LowLatency = int.Parse(lowLatency);
            }

            var h264_stap_a = data["rtp"]["h264_stap_a"];
            if (h264_stap_a != null && !string.IsNullOrEmpty(h264_stap_a) && UtilsHelper.IsInteger(h264_stap_a))
            {
                Rtp.H264_Stap_A = int.Parse(h264_stap_a);
            }

            #endregion

            #region rtp_proxy 部分

            if (Rtp_Proxy == null)
            {
                Rtp_Proxy = new ZLMediaKitConfigNew_Rtp_Proxy();
            }

            var dumpDir = data["rtp_proxy"]["dumpDir"];
            if (dumpDir != null && !string.IsNullOrEmpty(dumpDir))
            {
                Rtp_Proxy.DumpDir = dumpDir.Trim();
            }

            var rtp_proxy_port = data["rtp_proxy"]["port"];
            if (rtp_proxy_port != null && !string.IsNullOrEmpty(rtp_proxy_port) &&
                UtilsHelper.IsUShort(rtp_proxy_port))
            {
                Rtp_Proxy.Port = ushort.Parse(rtp_proxy_port);
            }

            var rtp_proxy_timeoutSec = data["rtp_proxy"]["timeoutSec"];
            if (rtp_proxy_timeoutSec != null && !string.IsNullOrEmpty(rtp_proxy_timeoutSec) &&
                UtilsHelper.IsInteger(rtp_proxy_timeoutSec))
            {
                Rtp_Proxy.TimeoutSec = int.Parse(rtp_proxy_timeoutSec);
            }

            var port_range = data["rtp_proxy"]["port_range"];
            if (port_range != null && !string.IsNullOrEmpty(port_range))
            {
                Rtp_Proxy.Port_Range = port_range.Trim();
            }

            var h264_pt = data["rtp_proxy"]["h264_pt"];
            if (h264_pt != null && !string.IsNullOrEmpty(h264_pt))
            {
                Rtp_Proxy.H264_Pt = h264_pt.Trim();
            }

            var h265_pt = data["rtp_proxy"]["h265_pt"];
            if (h265_pt != null && !string.IsNullOrEmpty(h265_pt))
            {
                Rtp_Proxy.H265_Pt = h265_pt.Trim();
            }

            var ps_pt = data["rtp_proxy"]["ps_pt"];
            if (ps_pt != null && !string.IsNullOrEmpty(ps_pt))
            {
                Rtp_Proxy.Ps_Pt = ps_pt.Trim();
            }

            var opus_pt = data["rtp_proxy"]["opus_pt"];
            if (opus_pt != null && !string.IsNullOrEmpty(opus_pt))
            {
                Rtp_Proxy.Opus_Pt = opus_pt.Trim();
            }

            var gop_cache = data["rtp_proxy"]["gop_cache"];
            if (gop_cache != null && !string.IsNullOrEmpty(gop_cache) && UtilsHelper.IsInteger(gop_cache))
            {
                Rtp_Proxy.Gop_Cache = int.Parse(gop_cache);
            }

            #endregion

            #region rtc部分

            if (Rtc == null)
            {
                Rtc = new ZLMediaKitConfigNew_RTC();
            }

            var rtc_timeoutSec = data["rtc"]["timeoutSec"];
            if (rtc_timeoutSec != null && !string.IsNullOrEmpty(rtc_timeoutSec) &&
                UtilsHelper.IsInteger(rtc_timeoutSec))
            {
                Rtc.TimeoutSec = int.Parse(rtc_timeoutSec);
            }

            var externIP = data["rtc"]["externIP"];
            if (externIP != null && !string.IsNullOrEmpty(externIP))
            {
                Rtc.ExternIp = externIP.Trim();
            }

            var rtc_port = data["rtc"]["port"];
            if (rtc_port != null && !string.IsNullOrEmpty(rtc_port) && UtilsHelper.IsUShort(rtc_port))
            {
                Rtc.Port = ushort.Parse(rtc_port);
            }

            var tcpPort = data["rtc"]["tcpPort"];
            if (tcpPort != null && !string.IsNullOrEmpty(tcpPort) && UtilsHelper.IsUShort(tcpPort))
            {
                Rtc.TcpPort = ushort.Parse(tcpPort);
            }

            var rembBitRate = data["rtc"]["rembBitRate"];
            if (rembBitRate != null && !string.IsNullOrEmpty(rembBitRate) && UtilsHelper.IsInteger(rembBitRate))
            {
                Rtc.RembBitRate = int.Parse(rembBitRate);
            }

            var preferredCodecA = data["rtc"]["preferredCodecA"];
            if (preferredCodecA != null && !string.IsNullOrEmpty(preferredCodecA))
            {
                Rtc.PreferredCodecA = preferredCodecA.Trim();
            }

            var preferredCodecV = data["rtc"]["preferredCodecV"];
            if (preferredCodecV != null && !string.IsNullOrEmpty(preferredCodecV))
            {
                Rtc.PreferredCodecV = preferredCodecV.Trim();
            }

            #endregion

            #region srt部分

            if (Srt == null)
            {
                Srt = new ZLMediaKitConfigNew_SRT();
            }

            var srt_timeoutSec = data["srt"]["timeoutSec"];
            if (srt_timeoutSec != null && !string.IsNullOrEmpty(srt_timeoutSec) &&
                UtilsHelper.IsInteger(srt_timeoutSec))
            {
                Srt.TimeoutSec = int.Parse(srt_timeoutSec);
            }

            var srtPort = data["srt"]["port"];
            if (srtPort != null && !string.IsNullOrEmpty(srtPort) && UtilsHelper.IsUShort(srtPort))
            {
                Srt.Port = ushort.Parse(srtPort);
            }

            var latencyMul = data["srt"]["latencyMul"];
            if (latencyMul != null && !string.IsNullOrEmpty(latencyMul) && UtilsHelper.IsInteger(latencyMul))
            {
                Srt.LatencyMul = int.Parse(latencyMul);
            }

            var pktBufSize = data["srt"]["pktBufSize"];
            if (pktBufSize != null && !string.IsNullOrEmpty(pktBufSize) && UtilsHelper.IsInteger(pktBufSize))
            {
                Srt.PktBufSize = int.Parse(pktBufSize);
            }

            #endregion

            #region rtsp部分

            if (Rtsp == null)
            {
                Rtsp = new ZLMediaKitConfigNew_RTSP();
            }

            var authBasic = data["rtsp"]["authBasic"];
            if (authBasic != null && !string.IsNullOrEmpty(authBasic) && UtilsHelper.IsInteger(authBasic))
            {
                Rtsp.AuthBasic = int.Parse(authBasic);
            }

            var directProxy = data["rtsp"]["directProxy"];
            if (directProxy != null && !string.IsNullOrEmpty(directProxy) && UtilsHelper.IsInteger(directProxy))
            {
                Rtsp.DirectProxy = int.Parse(directProxy);
            }

            var rtsp_handshakeSecond = data["rtsp"]["handshakeSecond"];
            if (rtsp_handshakeSecond != null && !string.IsNullOrEmpty(rtsp_handshakeSecond) &&
                UtilsHelper.IsInteger(rtsp_handshakeSecond))
            {
                Rtsp.HandshakeSecond = int.Parse(rtsp_handshakeSecond);
            }

            var rtsp_keepAliveSecond = data["rtsp"]["keepAliveSecond"];
            if (rtsp_keepAliveSecond != null && !string.IsNullOrEmpty(rtsp_keepAliveSecond) &&
                UtilsHelper.IsInteger(rtsp_keepAliveSecond))
            {
                Rtsp.KeepAliveSecond = int.Parse(rtsp_keepAliveSecond);
            }

            var rtsp_port = data["rtsp"]["port"];
            if (rtsp_port != null && !string.IsNullOrEmpty(rtsp_port) && UtilsHelper.IsUShort(rtsp_port))
            {
                Rtsp.Port = ushort.Parse(rtsp_port);
            }

            var rtsp_sslport = data["rtsp"]["sslport"];
            if (rtsp_sslport != null && !string.IsNullOrEmpty(rtsp_sslport) &&
                UtilsHelper.IsUShort(rtsp_sslport))
            {
                Rtsp.Sslport = ushort.Parse(rtsp_sslport);
            }

            var rtsp_lowLatency = data["rtsp"]["lowLatency"];
            if (rtsp_lowLatency != null && !string.IsNullOrEmpty(rtsp_lowLatency) &&
                UtilsHelper.IsInteger(rtsp_lowLatency))
            {
                Rtsp.LowLatency = int.Parse(rtsp_lowLatency);
            }

            var rtpTransportType = data["rtsp"]["rtpTransportType"];
            if (rtpTransportType != null && !string.IsNullOrEmpty(rtpTransportType) &&
                UtilsHelper.IsInteger(rtpTransportType))
            {
                Rtsp.RtpTransportType = int.Parse(rtpTransportType);
            }

            #endregion

            #region shell部分

            if (Shell == null)
            {
                Shell = new ZLMediaKitConfigNew_Shell();
            }

            var shell_maxReqSize = data["shell"]["maxReqSize"];
            if (shell_maxReqSize != null && !string.IsNullOrEmpty(shell_maxReqSize) &&
                UtilsHelper.IsInteger(shell_maxReqSize))
            {
                Shell.MaxReqSize = int.Parse(shell_maxReqSize);
            }

            var shell_port = data["shell"]["port"];
            if (shell_port != null && !string.IsNullOrEmpty(shell_port) && UtilsHelper.IsUShort(shell_port))
            {
                Shell.Port = ushort.Parse(shell_port);
            }

            #endregion
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}