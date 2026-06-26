using System;
using LibCommon;
using Newtonsoft.Json;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest;

[Serializable]
public class ReqForWebHookOnServerStarted
{
    private bool? _api_apiDebug;
    private string? _api_defaultSnap;
    private string? _api_secret;
    private string? _api_snapRoot;
    private string? _cluster_origin_url;
    private int? _cluster_retry_count;
    private int? _cluster_timeout_sec;
    private string? _ffmpeg_bin;
    private string? _ffmpeg_cmd;
    private string? _ffmpeg_log;
    private int? _ffmpeg_restart_sec;
    private string? _ffmpeg_snap;
    private string? _ffmpeg_templete_ffmpeg2flv;
    private string? _ffmpeg_templete_rtsp_tcp2flv;
    private bool? _general_check_nvidia_dev;
    private bool? _general_enableVhost;
    private bool? _general_enable_ffmpeg_log;
    private int? _general_flowThreshold;
    private int? _general_maxStreamWaitMS;
    private string? _general_mediaServerId;
    private int? _general_mergeWriteMS;
    private bool? _general_resetWhenRePlay;
    private int? _general_streamNoneReaderDelayMS;
    private int? _general_unready_frame_cache;
    private int? _general_wait_add_track_ms;
    private int? _general_wait_track_ready_ms;
    private bool? _hls_broadcastRecordTs;
    private int? _hls_deleteDelaySec;
    private int? _hls_fileBufSize;
    private int? _hls_segDur;
    private int? _hls_segKeep;
    private int? _hls_segNum;
    private int? _hls_segRetain;
    private string? _hook_admin_params;
    private float? _hook_alive_interval;
    private bool? _hook_enable;
    private string? _hook_on_flow_report;
    private string? _hook_on_http_access;
    private string? _hook_on_play;
    private string? _hook_on_publish;
    private string? _hook_on_record_mp4;
    private string? _hook_on_record_ts;
    private string? _hook_on_rtp_server_timeout;
    private string? _hook_on_rtsp_auth;
    private string? _hook_on_rtsp_realm;
    private string? _hook_on_send_rtp_stopped;
    private string? _hook_on_server_keepalive;
    private string? _hook_on_server_started;
    private string? _hook_on_shell_login;
    private string? _hook_on_stream_changed;
    private string? _hook_on_stream_none_reader;
    private string? _hook_on_stream_not_found;
    private int? _hook_retry;
    private float? _hook_retry_delay;
    private int? _hook_timeoutSec;
    private bool? _http_allow_cross_domains;
    private string? _http_charSet;
    private bool? _http_dirMenu;
    private string? _http_forbidCacheSuffix;
    private string? _http_forwarded_ip_header;
    private int? _http_keepAliveSecond;
    private int? _http_maxReqSize;
    private string? _http_notFound;
    private ushort? _http_port;
    private string? _http_rootPath;
    private int? _http_sendBufSize;
    private ushort? _http_sslport;
    private string? _http_virtualPath;
    private string? _mediaServerId;
    private string? _multicast_addrMax;
    private string? _multicast_addrMin;
    private int? _multicast_udpTTL;
    private bool? _protocol_add_mute_audio;
    private int? _protocol_continue_push_ms;
    private bool? _protocol_enable_audio;
    private bool? _protocol_enable_fmp4;
    private bool? _protocol_enable_hls;
    private bool? _protocol_enable_mp4;
    private bool? _protocol_enable_rtmp;
    private bool? _protocol_enable_rtsp;
    private bool? _protocol_enable_ts;
    private bool? _protocol_fmp4_demand;
    private bool? _protocol_hls_demand;
    private string? _protocol_hls_save_path;
    private int? _protocol_modify_stamp;
    private bool? _protocol_mp4_as_player;
    private int? _protocol_mp4_max_second;
    private string? _protocol_mp4_save_path;
    private bool? _protocol_rtmp_demand;
    private bool? _protocol_rtsp_demand;
    private bool? _protocol_ts_demand;
    private string? _record_appName;
    private bool? _record_fastStart;
    private int? _record_fileBufSize;
    private bool? _record_fileRepeat;
    private int? _record_sampleMS;
    private string? _rtc_externIP;
    private ushort? _rtc_port;
    private string? _rtc_preferredCodecA;
    private string? _rtc_preferredCodecV;
    private int? _rtc_rembBitRate;
    private ushort? _rtc_tcpPort;
    private int? _rtc_timeoutSec;
    private int? _rtmp_handshakeSecond;
    private int? _rtmp_keepAliveSecond;
    private bool? _rtmp_modifyStamp;
    private ushort? _rtmp_port;
    private ushort? _rtmp_sslport;
    private int? _rtp_audioMtuSize;
    private bool? _rtp_lowLatency;
    private int? _rtp_rtpMaxSize;
    private int? _rtp_videoMtuSize;
    private string? _rtp_Proxy_dumpDir;
    private bool? _rtp_proxy_gop_cache;
    private string? _rtp_proxy_h264_pt;
    private string? _rtp_proxy_h265_pt;
    private string? _rtp_proxy_opus_pt;
    private ushort? _rtp_proxy_port;
    private string? _rtp_proxy_port_range;
    private string? _rtp_proxy_ps_pt;
    private int? _rtp_proxy_timeoutSec;
    private bool? _rtsp_authBasic;
    private bool? _rtsp_directProxy;
    private int? _rtsp_handshakeSecond;
    private int? _rtsp_keepAliveSecond;
    private bool? _rtsp_lowLatency;
    private ushort? _rtsp_port;
    private ushort? _rtsp_sslport;
    private int? _shell_maxReqSize;
    private ushort? _shell_port;
    private int? _srt_latencyMul;
    private int? _srt_pktBufSize;
    private ushort? _srt_port;
    private int? _srt_timeoutSec;

    [JsonProperty("mediaServerId")]
    public string? MediaServerId
    {
        get => _mediaServerId;
        set => _mediaServerId = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("api.apiDebug")]
    public bool? Api_ApiDebug
    {
        get => _api_apiDebug;
        set => _api_apiDebug = value;
    }

    [JsonProperty("api.defaultSnap")]
    public string? Api_DefaultSnap
    {
        get => _api_defaultSnap;
        set => _api_defaultSnap = value;
    }


    [JsonProperty("api.secret")]
    public string? Api_Secret
    {
        get => _api_secret;
        set => _api_secret = value;
    }

    [JsonProperty("api.snapRoot")]
    public string? Api_SnapRoot
    {
        get => _api_snapRoot;
        set => _api_snapRoot = value;
    }

    [JsonProperty("cluster.origin_url")]
    public string Cluster_Origin_Url
    {
        get => _cluster_origin_url;
        set => _cluster_origin_url = value;
    }

    [JsonProperty("cluster.retry_count")]
    public int? Cluster_Retry_Count
    {
        get => _cluster_retry_count;
        set => _cluster_retry_count = value;
    }

    [JsonProperty("cluster.timeout_sec")]
    public int? Cluster_Timeout_Sec
    {
        get => _cluster_timeout_sec;
        set => _cluster_timeout_sec = value;
    }

    [JsonProperty("ffmpeg.bin")]
    public string? Ffmpeg_Bin
    {
        get => _ffmpeg_bin;
        set => _ffmpeg_bin = value;
    }

    [JsonProperty("ffmpeg.cmd")]
    public string? Ffmpeg_Cmd
    {
        get => _ffmpeg_cmd;
        set => _ffmpeg_cmd = value;
    }

    [JsonProperty("ffmpeg.log")]
    public string? Ffmpeg_Log
    {
        get => _ffmpeg_log;
        set => _ffmpeg_log = value;
    }

    [JsonProperty("ffmpeg.snap")]
    public string? Ffmpeg_Snap
    {
        get => _ffmpeg_snap;
        set => _ffmpeg_snap = value;
    }

    [JsonProperty("ffmpeg.restart_sec")]
    public int? FFmpeg_Restart_Sec
    {
        get => _ffmpeg_restart_sec;
        set => _ffmpeg_restart_sec = value;
    }

    [JsonProperty("ffmpeg_templete.ffmpeg2flv")]
    public string? Ffmpeg_Templete_Ffmpeg2Flv
    {
        get => _ffmpeg_templete_ffmpeg2flv;
        set => _ffmpeg_templete_ffmpeg2flv = value;
    }

    [JsonProperty("ffmpeg_templete.rtsp_tcp2flv")]
    public string? Ffmpeg_Templete_RtspTcp2Flv
    {
        get => _ffmpeg_templete_rtsp_tcp2flv;
        set => _ffmpeg_templete_rtsp_tcp2flv = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("general.check_nvidia_dev")]
    public bool? General_Check_Nvidia_Dev
    {
        get => _general_check_nvidia_dev;
        set => _general_check_nvidia_dev = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("general.enable_ffmpeg_log")]
    public bool? General_Enable_FFmpeg_Log
    {
        get => _general_enable_ffmpeg_log;
        set => _general_enable_ffmpeg_log = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("general.enableVhost")]
    public bool? General_EnableVhost
    {
        get => _general_enableVhost;
        set => _general_enableVhost = value;
    }

    [JsonProperty("general.flowThreshold")]
    public int? General_FlowThreshold
    {
        get => _general_flowThreshold;
        set => _general_flowThreshold = value;
    }


    [JsonProperty("general.maxStreamWaitMS")]
    public int? General_MaxStreamWaitMs
    {
        get => _general_maxStreamWaitMS;
        set => _general_maxStreamWaitMS = value;
    }

    [JsonProperty("general.mediaServerId")]
    public string? General_MediaServerId
    {
        get => _general_mediaServerId;
        set => _general_mediaServerId = value;
    }

    [JsonProperty("general.mergeWriteMs")]
    public int? General_MergeWriteMs
    {
        get => _general_mergeWriteMS;
        set => _general_mergeWriteMS = value;
    }


    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("general.resetWhenRePlay")]
    public bool? General_ResetWhenRePlay
    {
        get => _general_resetWhenRePlay;
        set => _general_resetWhenRePlay = value;
    }

    [JsonProperty("general.streamNoneReaderDelayMS")]
    public int? General_StreamNoneReaderDelayMs
    {
        get => _general_streamNoneReaderDelayMS;
        set => _general_streamNoneReaderDelayMS = value;
    }


    [JsonProperty("general.unready_frame_cache")]

    public int? General_Unready_Frame_Cache
    {
        get => _general_unready_frame_cache;
        set => _general_unready_frame_cache = value;
    }

    [JsonProperty("general.wait_add_track_ms")]
    public int? General_Wait_Add_Track_Ms
    {
        get => _general_wait_add_track_ms;
        set => _general_wait_add_track_ms = value;
    }

    [JsonProperty("general.wait_track_ready_ms")]
    public int? General_Wait_Track_Ready_Ms
    {
        get => _general_wait_track_ready_ms;
        set => _general_wait_track_ready_ms = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("hls.broadcastRecordTs")]
    public bool? Hls_BroadcastRecordTs
    {
        get => _hls_broadcastRecordTs;
        set => _hls_broadcastRecordTs = value;
    }

    [JsonProperty("hls.deleteDelaySec")]
    public int? Hls_DeleteDelaySec
    {
        get => _hls_deleteDelaySec;
        set => _hls_deleteDelaySec = value;
    }

    [JsonProperty("hls.fileBufSize")]
    public int? Hls_FileBufSize
    {
        get => _hls_fileBufSize;
        set => _hls_fileBufSize = value;
    }


    [JsonProperty("hls.segDur")]
    public int? Hls_SegDur
    {
        get => _hls_segDur;
        set => _hls_segDur = value;
    }

    [JsonProperty("hls.segKeep")]
    public int? Hls_SegKeep
    {
        get => _hls_segKeep;
        set => _hls_segKeep = value;
    }

    [JsonProperty("hls.segNum")]
    public int? Hls_SegNum
    {
        get => _hls_segNum;
        set => _hls_segNum = value;
    }

    [JsonProperty("hls.segRetain")]
    public int? Hls_SegRetain
    {
        get => _hls_segRetain;
        set => _hls_segRetain = value;
    }

    [JsonProperty("hook.admin_params")]
    public string? Hook_Admin_Params
    {
        get => _hook_admin_params;
        set => _hook_admin_params = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("hook.enable")]
    public bool? Hook_Enable
    {
        get => _hook_enable;
        set => _hook_enable = value;
    }

    [JsonProperty("hook.on_flow_report")]
    public string? Hook_On_Flow_Report
    {
        get => _hook_on_flow_report;
        set => _hook_on_flow_report = value;
    }

    [JsonProperty("hook.on_http_access")]
    public string? Hook_On_Http_Access
    {
        get => _hook_on_http_access;
        set => _hook_on_http_access = value;
    }

    [JsonProperty("hook.on_play")]
    public string? Hook_On_Play
    {
        get => _hook_on_play;
        set => _hook_on_play = value;
    }

    [JsonProperty("hook.on_publish")]
    public string? Hook_On_Publish
    {
        get => _hook_on_publish;
        set => _hook_on_publish = value;
    }

    [JsonProperty("hook.on_record_mp4")]
    public string? Hook_On_Record_Mp4
    {
        get => _hook_on_record_mp4;
        set => _hook_on_record_mp4 = value;
    }

    [JsonProperty("hook.on_record_ts")]
    public string? Hook_On_Record_Ts
    {
        get => _hook_on_record_ts;
        set => _hook_on_record_ts = value;
    }

    [JsonProperty("hook.on_rtsp_auth")]
    public string? Hook_On_Rtsp_Auth
    {
        get => _hook_on_rtsp_auth;
        set => _hook_on_rtsp_auth = value;
    }

    [JsonProperty("hook.on_rtsp_realm")]
    public string? HookOnRtspRealm
    {
        get => _hook_on_rtsp_realm;
        set => _hook_on_rtsp_realm = value;
    }

    [JsonProperty("hook.on_server_started")]
    public string? Hook_On_Server_Started
    {
        get => _hook_on_server_started;
        set => _hook_on_server_started = value;
    }

    [JsonProperty("hook.on_shell_login")]
    public string? Hook_On_Shell_Login
    {
        get => _hook_on_shell_login;
        set => _hook_on_shell_login = value;
    }

    [JsonProperty("hook.on_stream_changed")]
    public string? Hook_On_Stream_Changed
    {
        get => _hook_on_stream_changed;
        set => _hook_on_stream_changed = value;
    }

    [JsonProperty("hook.on_stream_none_reader")]
    public string? Hook_On_Stream_None_Reader
    {
        get => _hook_on_stream_none_reader;
        set => _hook_on_stream_none_reader = value;
    }

    [JsonProperty("hook.on_stream_not_found")]
    public string? Hook_On_Stream_Not_Found
    {
        get => _hook_on_stream_not_found;
        set => _hook_on_stream_not_found = value;
    }

    [JsonProperty("hook.timeoutSec")]
    public int? Hook_TimeoutSec
    {
        get => _hook_timeoutSec;
        set => _hook_timeoutSec = value;
    }

    [JsonProperty("hook.retry")]
    public int? Hook_Retry
    {
        get => _hook_retry;
        set => _hook_retry = value;
    }

    [JsonProperty("hook.retry_delay")]
    public float? Hook_Retry_Delay
    {
        get => _hook_retry_delay;
        set => _hook_retry_delay = value;
    }

    [JsonProperty("hook.on_send_rtp_stopped")]
    public string Hook_On_Send_Rtp_Stopped
    {
        get => _hook_on_send_rtp_stopped;
        set => _hook_on_send_rtp_stopped = value;
    }

    [JsonProperty("hook.on_server_keepalive")]
    public string Hook_On_Server_Keepalive
    {
        get => _hook_on_server_keepalive;
        set => _hook_on_server_keepalive = value;
    }

    [JsonProperty("hook.on_rtp_server_timeout")]
    public string Hook_On_Rtp_Server_Timeout
    {
        get => _hook_on_rtp_server_timeout;
        set => _hook_on_rtp_server_timeout = value;
    }

    [JsonProperty("hook.alive_interval")]
    public float? Hook_Alive_Interval
    {
        get => _hook_alive_interval;
        set => _hook_alive_interval = value;
    }


    [JsonProperty("http.charSet")]
    public string? Http_CharSet
    {
        get => _http_charSet;
        set => _http_charSet = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("http.dirMenu")]
    public bool? Http_DirMenu
    {
        get => _http_dirMenu;
        set => _http_dirMenu = value;
    }

    [JsonProperty("http.keepAliveSecond")]
    public int? Http_KeepAliveSecond
    {
        get => _http_keepAliveSecond;
        set => _http_keepAliveSecond = value;
    }

    [JsonProperty("http.maxReqSize")]
    public int? Http_MaxReqSize
    {
        get => _http_maxReqSize;
        set => _http_maxReqSize = value;
    }

    [JsonProperty("http.notFound")]
    public string? Http_NotFound
    {
        get => _http_notFound;
        set => _http_notFound = value;
    }

    [JsonProperty("http.port")]
    public ushort? Http_Port
    {
        get => _http_port;
        set => _http_port = value;
    }

    [JsonProperty("http.rootPath")]
    public string? Http_RootPath
    {
        get => _http_rootPath;
        set => _http_rootPath = value;
    }

    [JsonProperty("http.sendBufSize")]
    public int? Http_SendBufSize
    {
        get => _http_sendBufSize;
        set => _http_sendBufSize = value;
    }

    [JsonProperty("http.sslport")]
    public ushort? Http_Sslport
    {
        get => _http_sslport;
        set => _http_sslport = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("http.allow_cross_domains")]
    public bool? Http_Allow_Cross_Domains
    {
        get => _http_allow_cross_domains;
        set => _http_allow_cross_domains = value;
    }

    [JsonProperty("http.forbidCacheSuffix")]
    public string Http_ForbidCacheSuffix
    {
        get => _http_forbidCacheSuffix;
        set => _http_forbidCacheSuffix = value;
    }

    [JsonProperty("http.forwarded_ip_header")]
    public string Http_Forwarded_Ip_Header
    {
        get => _http_forwarded_ip_header;
        set => _http_forwarded_ip_header = value;
    }

    [JsonProperty("http.virtualPath")]
    public string Http_VirtualPath
    {
        get => _http_virtualPath;
        set => _http_virtualPath = value;
    }

    [JsonProperty("multicast.addrMax")]
    public string? Multicast_AddrMax
    {
        get => _multicast_addrMax;
        set => _multicast_addrMax = value;
    }

    [JsonProperty("multicast.addrMin")]
    public string? Multicast_AddrMin
    {
        get => _multicast_addrMin;
        set => _multicast_addrMin = value;
    }

    [JsonProperty("multicast.udpTTL")]
    public int? Multicast_UdpTtl
    {
        get => _multicast_udpTTL;
        set => _multicast_udpTTL = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.add_mute_audio")]
    public bool? Protocol_AddMute_Audio
    {
        get => _protocol_add_mute_audio;
        set => _protocol_add_mute_audio = value;
    }

    [JsonProperty("protocol.continue_push_ms")]
    public int? Protocol_Continue_Push_Ms
    {
        get => _protocol_continue_push_ms;
        set => _protocol_continue_push_ms = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_audio")]
    public bool? Protocol_Enable_Audio
    {
        get => _protocol_enable_audio;
        set => _protocol_enable_audio = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_fmp4")]
    public bool? Protocol_Enable_Fmp4
    {
        get => _protocol_enable_fmp4;
        set => _protocol_enable_fmp4 = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_hls")]
    public bool? Protocol_Enable_Hls
    {
        get => _protocol_enable_hls;
        set => _protocol_enable_hls = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_mp4")]
    public bool? Protocol_Enable_Mp4
    {
        get => _protocol_enable_mp4;
        set => _protocol_enable_mp4 = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_rtmp")]
    public bool? Protocol_Enable_Rtmp
    {
        get => _protocol_enable_rtmp;
        set => _protocol_enable_rtmp = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_rtsp")]
    public bool? Protocol_Enable_Rtsp
    {
        get => _protocol_enable_rtsp;
        set => _protocol_enable_rtsp = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.enable_ts")]
    public bool? Protocol_Enable_Ts
    {
        get => _protocol_enable_ts;
        set => _protocol_enable_ts = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.fmp4_demand")]
    public bool? Protocol_Fmp4_Demand
    {
        get => _protocol_fmp4_demand;
        set => _protocol_fmp4_demand = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.hls_demand")]
    public bool? Protocol_Hls_Demand
    {
        get => _protocol_hls_demand;
        set => _protocol_hls_demand = value;
    }

    [JsonProperty("protocol.hls_save_path")]
    public string Protocol_Hls_Save_Path
    {
        get => _protocol_hls_save_path;
        set => _protocol_hls_save_path = value;
    }

    [JsonProperty("protocol.modify_stamp")]
    public int? Protocol_Modify_Stamp
    {
        get => _protocol_modify_stamp;
        set => _protocol_modify_stamp = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.mp4_as_player")]
    public bool? Protocol_Mp4_As_Player
    {
        get => _protocol_mp4_as_player;
        set => _protocol_mp4_as_player = value;
    }

    [JsonProperty("protocol.mp4_max_second")]
    public int? Protocol_Mp4_Max_Second
    {
        get => _protocol_mp4_max_second;
        set => _protocol_mp4_max_second = value;
    }

    [JsonProperty("protocol.mp4_save_path")]
    public string Protocol_Mp4_Save_Path
    {
        get => _protocol_mp4_save_path;
        set => _protocol_mp4_save_path = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.rtmp_demand")]
    public bool? Protocol_Rtmp_Demand
    {
        get => _protocol_rtmp_demand;
        set => _protocol_rtmp_demand = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.rtsp_demand")]
    public bool? Protocol_Rtsp_Demand
    {
        get => _protocol_rtsp_demand;
        set => _protocol_rtsp_demand = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("protocol.ts_demand")]
    public bool? Protocol_Ts_Demand
    {
        get => _protocol_ts_demand;
        set => _protocol_ts_demand = value;
    }

    [JsonProperty("record.appName")]
    public string? Record_AppName
    {
        get => _record_appName;
        set => _record_appName = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("record.fastStart")]
    public bool? Record_FastStart
    {
        get => _record_fastStart;
        set => _record_fastStart = value;
    }

    [JsonProperty("record.fileBufSize")]
    public int? Record_FileBufSize
    {
        get => _record_fileBufSize;
        set => _record_fileBufSize = value;
    }


    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("record.fileRepeat")]
    public bool? Record_FileRepeat
    {
        get => _record_fileRepeat;
        set => _record_fileRepeat = value;
    }


    [JsonProperty("record.sampleMS")]
    public int? Record_SampleMs
    {
        get => _record_sampleMS;
        set => _record_sampleMS = value;
    }

    [JsonProperty("rtc.externIP")]
    public string Rtc_ExternIp
    {
        get => _rtc_externIP;
        set => _rtc_externIP = value;
    }

    [JsonProperty("rtc.port")]
    public ushort? Rtc_Port
    {
        get => _rtc_port;
        set => _rtc_port = value;
    }

    [JsonProperty("rtc.preferredCodecA")]
    public string Rtc_PreferredCodecA
    {
        get => _rtc_preferredCodecA;
        set => _rtc_preferredCodecA = value;
    }

    [JsonProperty("rtc.preferredCodecV")]
    public string Rtc_PreferredCodecV
    {
        get => _rtc_preferredCodecV;
        set => _rtc_preferredCodecV = value;
    }

    [JsonProperty("rtc.rembBitRate")]
    public int? Rtc_RembBitRate
    {
        get => _rtc_rembBitRate;
        set => _rtc_rembBitRate = value;
    }

    [JsonProperty("rtc.tcpPort")]
    public ushort? Rtc_TcpPort
    {
        get => _rtc_tcpPort;
        set => _rtc_tcpPort = value;
    }

    [JsonProperty("rtc.timeoutSec")]
    public int? Rtc_TimeoutSec
    {
        get => _rtc_timeoutSec;
        set => _rtc_timeoutSec = value;
    }

    [JsonProperty("rtmp.handshakeSecond")]
    public int? Rtmp_HandshakeSecond
    {
        get => _rtmp_handshakeSecond;
        set => _rtmp_handshakeSecond = value;
    }

    [JsonProperty("rtmp.keepAliveSecond")]
    public int? Rtmp_KeepAliveSecond
    {
        get => _rtmp_keepAliveSecond;
        set => _rtmp_keepAliveSecond = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtmp.modifyStamp")]
    public bool? Rtmp_ModifyStamp
    {
        get => _rtmp_modifyStamp;
        set => _rtmp_modifyStamp = value;
    }

    [JsonProperty("rtmp.port")]
    public ushort? Rtmp_Port
    {
        get => _rtmp_port;
        set => _rtmp_port = value;
    }

    [JsonProperty("rtmp.sslport")]
    public ushort? Rtmp_Sslport
    {
        get => _rtmp_sslport;
        set => _rtmp_sslport = value;
    }

    [JsonProperty("rtp.audioMtuSize")]
    public int? Rtp_AudioMtuSize
    {
        get => _rtp_audioMtuSize;
        set => _rtp_audioMtuSize = value;
    }


    [JsonProperty("rtp.videoMtuSize")]
    public int? Rtp_VideoMtuSize
    {
        get => _rtp_videoMtuSize;
        set => _rtp_videoMtuSize = value;
    }


    [JsonProperty("rtp_proxy.dumpDir")]
    public string? Rtp_Proxy_DumpDir
    {
        get => _rtp_Proxy_dumpDir;
        set => _rtp_Proxy_dumpDir = value;
    }

    [JsonProperty("rtp_proxy.port")]
    public ushort? Rtp_Proxy_Port
    {
        get => _rtp_proxy_port;
        set => _rtp_proxy_port = value;
    }

    [JsonProperty("rtp_proxy.timeoutSec")]
    public int? Rtp_Proxy_TimeoutSec
    {
        get => _rtp_proxy_timeoutSec;
        set => _rtp_proxy_timeoutSec = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtp.lowLatency")]
    public bool? Rtp_LowLatency
    {
        get => _rtp_lowLatency;
        set => _rtp_lowLatency = value;
    }

    [JsonProperty("rtp.rtpMaxSize")]
    public int? Rtp_RtpMaxSize
    {
        get => _rtp_rtpMaxSize;
        set => _rtp_rtpMaxSize = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtp_proxy.gop_cache")]
    public bool? Rtp_Proxy_Gop_Cache
    {
        get => _rtp_proxy_gop_cache;
        set => _rtp_proxy_gop_cache = value;
    }

    [JsonProperty("rtp_proxy.h264_pt")]
    public string Rtp_Proxy_H264_Pt
    {
        get => _rtp_proxy_h264_pt;
        set => _rtp_proxy_h264_pt = value;
    }

    [JsonProperty("rtp_proxy.h265_pt")]
    public string Rtp_Proxy_H265_Pt
    {
        get => _rtp_proxy_h265_pt;
        set => _rtp_proxy_h265_pt = value;
    }

    [JsonProperty("rtp_proxy.opus_pt")]
    public string Rtp_Proxy_Opus_Pt
    {
        get => _rtp_proxy_opus_pt;
        set => _rtp_proxy_opus_pt = value;
    }

    [JsonProperty("rtp_proxy.ps_pt")]
    public string Rtp_Proxy_Ps_Pt
    {
        get => _rtp_proxy_ps_pt;
        set => _rtp_proxy_ps_pt = value;
    }

    [JsonProperty("rtp_proxy.port_range")]
    public string Rtp_Proxy_Port_Range
    {
        get => _rtp_proxy_port_range;
        set => _rtp_proxy_port_range = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtsp.authBasic")]
    public bool? Rtsp_AuthBasic
    {
        get => _rtsp_authBasic;
        set => _rtsp_authBasic = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtsp.directProxy")]
    public bool? RtspDirectProxy
    {
        get => _rtsp_directProxy;
        set => _rtsp_directProxy = value;
    }

    [JsonProperty("rtsp.handshakeSecond")]
    public int? Rtsp_HandshakeSecond
    {
        get => _rtsp_handshakeSecond;
        set => _rtsp_handshakeSecond = value;
    }

    [JsonProperty("rtsp.keepAliveSecond")]
    public int? Rtsp_KeepAliveSecond
    {
        get => _rtsp_keepAliveSecond;
        set => _rtsp_keepAliveSecond = value;
    }

    [JsonProperty("rtsp.port")]
    public ushort? Rtsp_Port
    {
        get => _rtsp_port;
        set => _rtsp_port = value;
    }

    [JsonProperty("rtsp.sslport")]
    public ushort? Rtsp_Sslport
    {
        get => _rtsp_sslport;
        set => _rtsp_sslport = value;
    }

    [JsonConverter(typeof(BoolConvert))]
    [JsonProperty("rtsp.lowLatency")]
    public bool? Rtsp_LowLatency
    {
        get => _rtsp_lowLatency;
        set => _rtsp_lowLatency = value;
    }


    [JsonProperty("shell.maxReqSize")]
    public int? Shell_MaxReqSize
    {
        get => _shell_maxReqSize;
        set => _shell_maxReqSize = value;
    }

    [JsonProperty("shell.port")]
    public ushort? Shell_Port
    {
        get => _shell_port;
        set => _shell_port = value;
    }

    [JsonProperty("srt.latencyMul")]
    public int? Srt_LatencyMul
    {
        get => _srt_latencyMul;
        set => _srt_latencyMul = value;
    }

    [JsonProperty("srt.pktBufSize")]
    public int? Srt_PktBufSize
    {
        get => _srt_pktBufSize;
        set => _srt_pktBufSize = value;
    }

    [JsonProperty("srt.port")]
    public ushort? Srt_Port
    {
        get => _srt_port;
        set => _srt_port = value;
    }

    [JsonProperty("srt.timeoutSec")]
    public int? Srt_TimeoutSec
    {
        get => _srt_timeoutSec;
        set => _srt_timeoutSec = value;
    }
}