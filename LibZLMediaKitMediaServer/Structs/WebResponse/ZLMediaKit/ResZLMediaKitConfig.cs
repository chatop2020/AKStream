using System;
using System.Collections.Generic;
using LibCommon;
using Newtonsoft.Json;

namespace LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit
{
    [Serializable]
    public class ZLMediaKitConfigForResponse
    {
        private bool? _api_apiDebug;
        private string? _api_defaultSnap;
        private string? _api_secret;
        private string? _api_snapRoot;
        private string? _ffmpeg_bin;
        private string? _ffmpeg_cmd;
        private string? _ffmpeg_log;
        private string? _ffmpeg_snap;
        private string? _ffmpeg_templete_ffmpeg2flv;
        private string? _ffmpeg_templete_rtsp_tcp2flv;
        private bool? _general_addMuteAudio;
        private bool? _general_enableVhost;
        private int? _general_flowThreshold;
        private bool? _general_fmp4_demand; //新增
        private bool? _general_hls_demand; //新增
        private int? _general_maxStreamWaitMS;
        private string? _general_mediaServerId;
        private int? _general_mergeWriteMS;
        private bool? _general_modifyStamp;
        private bool? _general_publishToHls;
        private bool? _general_publishToMP4;
        private bool? _general_resetWhenRePlay;
        private bool? _general_rtmp_demand; //新增
        private bool? _general_rtsp_demand; //新增
        private int? _general_streamNoneReaderDelayMS;
        private bool? _general_ts_demand; //新增
        private bool? _hls_broadcastRecordTs;
        private int? _hls_fileBufSize;
        private string? _hls_filePath;
        private int? _hls_segDur;
        private int? _hls_segNum;
        private int? _hls_segRetain;
        private string? _hook_admin_params;
        private bool? _hook_enable;
        private string? _hook_on_flow_report;
        private string? _hook_on_http_access;
        private string? _hook_on_play;
        private string? _hook_on_publish;
        private string? _hook_on_record_mp4;
        private string? _hook_on_record_ts;
        private string? _hook_on_rtsp_auth;
        private string? _hook_on_rtsp_realm;
        private string? _hook_on_server_started;
        private string? _hook_on_shell_login;
        private string? _hook_on_stream_changed;
        private string? _hook_on_stream_none_reader;
        private string? _hook_on_stream_not_found;
        private int? _hook_timeoutSec;
        private string? _http_charSet;
        private bool? _http_dirMenu;
        private int? _http_keepAliveSecond;
        private int? _http_maxReqSize;
        private string? _http_notFound;
        private ushort? _http_port;
        private string? _http_rootPath;
        private int? _http_sendBufSize;
        private ushort? _http_sslport;
        private string? _mediaServerId;
        private string? _multicast_addrMax;
        private string? _multicast_addrMin;
        private int? _multicast_udpTTL;
        private string? _record_appName;
        private bool? _record_fastStart;
        private int? _record_fileBufSize;
        private string? _record_filePath;
        private bool? _record_fileRepeat;
        private int? _record_fileSecond;
        private int? _record_sampleMS;
        private int? _rtmp_handshakeSecond;
        private int? _rtmp_keepAliveSecond;
        private bool? _rtmp_modifyStamp;
        private ushort? _rtmp_port;
        private ushort? _rtmp_sslport;
        private int? _rtp_audioMtuSize;
        private ulong? _rtp_cycleMS;
        private bool? _rtp_Proxy_checkSource;
        private string? _rtp_Proxy_dumpDir;
        private ushort? _rtp_Proxy_port;
        private int? _rtp_Proxy_timeoutSec;
        private int? _rtp_videoMtuSize;
        private bool? _rtsp_authBasic;
        private bool? _rtsp_directProxy;
        private int? _rtsp_handshakeSecond;
        private int? _rtsp_keepAliveSecond;
        private ushort? _rtsp_port;
        private ushort? _rtsp_sslport;
        private int? _shell_maxReqSize;
        private ushort? _shell_port;

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
        [JsonProperty("general.addMuteAudio")]
        public bool? General_AddMuteAudio
        {
            get => _general_addMuteAudio;
            set => _general_addMuteAudio = value;
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

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.fmp4_demand")]
        public bool? General_Fmp4_Demand
        {
            get => _general_fmp4_demand;
            set => _general_fmp4_demand = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.hls_demand")]
        public bool? General_Hls_Demand
        {
            get => _general_hls_demand;
            set => _general_hls_demand = value;
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
        [JsonProperty("general.modifyStamp")]
        public bool? General_ModifyStamp
        {
            get => _general_modifyStamp;
            set => _general_modifyStamp = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.publishToHls")]
        public bool? General_PublishToHls
        {
            get => _general_publishToHls;
            set => _general_publishToHls = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.publishToMp4")]
        public bool? General_PublishToMp4
        {
            get => _general_publishToMP4;
            set => _general_publishToMP4 = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.resetWhenRePlay")]
        public bool? General_ResetWhenRePlay
        {
            get => _general_resetWhenRePlay;
            set => _general_resetWhenRePlay = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.rtmp_demand")]
        public bool? General_Rtmp_Demand
        {
            get => _general_rtmp_demand;
            set => _general_rtmp_demand = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.rtsp_demand")]
        public bool? General_Rtsp_Demand
        {
            get => _general_rtsp_demand;
            set => _general_rtsp_demand = value;
        }

        [JsonProperty("general.streamNoneReaderDelayMS")]
        public int? General_StreamNoneReaderDelayMs
        {
            get => _general_streamNoneReaderDelayMS;
            set => _general_streamNoneReaderDelayMS = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("general.ts_demand")]
        public bool? General_Ts_Demand
        {
            get => _general_ts_demand;
            set => _general_ts_demand = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("hls.broadcastRecordTs")]
        public bool? Hls_BroadcastRecordTs
        {
            get => _hls_broadcastRecordTs;
            set => _hls_broadcastRecordTs = value;
        }

        [JsonProperty("hls.fileBufSize")]
        public int? Hls_FileBufSize
        {
            get => _hls_fileBufSize;
            set => _hls_fileBufSize = value;
        }

        [JsonProperty("hls.filePath")]
        public string? Hls_FilePath
        {
            get => _hls_filePath;
            set => _hls_filePath = value;
        }

        [JsonProperty("hls.segDur")]
        public int? Hls_SegDur
        {
            get => _hls_segDur;
            set => _hls_segDur = value;
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

        [JsonProperty("record.filePath")]
        public string? Record_FilePath
        {
            get => _record_filePath;
            set => _record_filePath = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("record.fileRepeat")]
        public bool? Record_FileRepeat
        {
            get => _record_fileRepeat;
            set => _record_fileRepeat = value;
        }

        [JsonProperty("record.fileSecond")]
        public int? Record_FileSecond
        {
            get => _record_fileSecond;
            set => _record_fileSecond = value;
        }

        [JsonProperty("record.sampleMS")]
        public int? Record_SampleMs
        {
            get => _record_sampleMS;
            set => _record_sampleMS = value;
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

       
        [JsonProperty("rtp.cycleMS")]
        public ulong? Rtp_CycleMs
        {
            get => _rtp_cycleMS;
            set => _rtp_cycleMS = value;
        }

      
        [JsonProperty("rtp.videoMtuSize")]
        public int? Rtp_VideoMtuSize
        {
            get => _rtp_videoMtuSize;
            set => _rtp_videoMtuSize = value;
        }

        [JsonConverter(typeof(BoolConvert))]
        [JsonProperty("rtp_proxy.checkSource")]
        public bool? Rtp_Proxy_CheckSource
        {
            get => _rtp_Proxy_checkSource;
            set => _rtp_Proxy_checkSource = value;
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
            get => _rtp_Proxy_port;
            set => _rtp_Proxy_port = value;
        }

        [JsonProperty("rtp_proxy.timeoutSec")]
        public int? Rtp_Proxy_TimeoutSec
        {
            get => _rtp_Proxy_timeoutSec;
            set => _rtp_Proxy_timeoutSec = value;
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
    }

    [Serializable]
    public class ResZLMediaKitConfig : ResZLMediaKitResponseBase
    {
        private List<ZLMediaKitConfigForResponse>? _data = null;


        public List<ZLMediaKitConfigForResponse>? Data
        {
            get => _data;
            set => _data = value;
        }
    }
}