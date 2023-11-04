using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_Hook
{
    private float? _alive_interval;
    private int? _enable;
    private string? _on_flow_report;
    private string? _on_http_access;
    private string? _on_play;
    private string? _on_publish;
    private string? _on_record_mp4;
    private string? _on_record_ts;
    private string? _on_rtp_server_timeout;
    private string? _on_rtsp_auth;
    private string? _on_rtsp_realm;
    private string? _on_send_rtp_stopped;
    private string? _on_server_exited;
    private string? _on_server_keepalive;
    private string? _on_server_started;
    private string? _on_shell_login;
    private string? _on_stream_changed;
    private string? _on_stream_none_reader;
    private string? _on_stream_not_found;
    private int? _retry;
    private float? _retry_delay;
    private int? _timeoutSec;


    /// <summary>
    /// 是否启用hook事件，启用后，推拉流都将进行鉴权
    /// </summary>
    public int? Enable
    {
        get => _enable;
        set => _enable = value;
    }

    /// <summary>
    /// 播放器或推流器使用流量事件，置空则关闭
    /// </summary>
    public string On_Flow_Report
    {
        get => _on_flow_report;
        set => _on_flow_report = value;
    }

    /// <summary>
    /// 访问http文件鉴权事件，置空则关闭鉴权
    /// </summary>
    public string On_Http_Access
    {
        get => _on_http_access;
        set => _on_http_access = value;
    }

    /// <summary>
    /// 播放鉴权事件，置空则关闭鉴权
    /// </summary>
    public string On_Play
    {
        get => _on_play;
        set => _on_play = value;
    }

    /// <summary>
    /// 推流鉴权事件，置空则关闭鉴权
    /// </summary>
    public string On_Publish
    {
        get => _on_publish;
        set => _on_publish = value;
    }

    /// <summary>
    /// 录制mp4切片完成事件
    /// </summary>
    public string On_Record_Mp4
    {
        get => _on_record_mp4;
        set => _on_record_mp4 = value;
    }

    /// <summary>
    /// 录制 hls ts 切片完成事件
    /// </summary>
    public string On_Record_Ts
    {
        get => _on_record_ts;
        set => _on_record_ts = value;
    }

    /// <summary>
    /// rtsp播放鉴权事件，此事件中比对rtsp的用户名密码
    /// </summary>
    public string On_Rtsp_Auth
    {
        get => _on_rtsp_auth;
        set => _on_rtsp_auth = value;
    }

    /// <summary>
    /// rtsp播放是否开启专属鉴权事件，置空则关闭rtsp鉴权。rtsp播放鉴权还支持url方式鉴权
    /// 建议开发者统一采用url参数方式鉴权，rtsp用户名密码鉴权一般在设备上用的比较多
    /// 开启rtsp专属鉴权后，将不再触发on_play鉴权事件
    /// </summary>
    public string On_Rtsp_Realm
    {
        get => _on_rtsp_realm;
        set => _on_rtsp_realm = value;
    }

    /// <summary>
    /// 远程telnet调试鉴权事件
    /// </summary>
    public string On_Shell_Login
    {
        get => _on_shell_login;
        set => _on_shell_login = value;
    }

    /// <summary>
    /// 直播流注册或注销事件
    /// </summary>
    public string On_Stream_Changed
    {
        get => _on_stream_changed;
        set => _on_stream_changed = value;
    }

    /// <summary>
    /// 无人观看流事件，通过该事件，可以选择是否关闭无人观看的流。配合general.streamNoneReaderDelayMS选项一起使用
    /// </summary>
    public string On_Stream_None_Reader
    {
        get => _on_stream_none_reader;
        set => _on_stream_none_reader = value;
    }

    /// <summary>
    /// 播放时，未找到流事件，通过配合hook.on_stream_none_reader事件可以完成按需拉流
    /// </summary>
    public string On_Stream_Not_Found
    {
        get => _on_stream_not_found;
        set => _on_stream_not_found = value;
    }

    /// <summary>
    /// 服务器启动报告，可以用于服务器的崩溃重启事件监听
    /// </summary>
    public string On_Server_Started
    {
        get => _on_server_started;
        set => _on_server_started = value;
    }

    /// <summary>
    /// server保活上报
    /// </summary>
    public string On_Server_Keepalive
    {
        get => _on_server_keepalive;
        set => _on_server_keepalive = value;
    }

    /// <summary>
    /// 发送rtp(startSendRtp)被动关闭时回调
    /// </summary>
    public string On_Send_Rtp_Stopped
    {
        get => _on_send_rtp_stopped;
        set => _on_send_rtp_stopped = value;
    }

    /// <summary>
    /// 服务器退出报告，当服务器正常退出时触发
    /// </summary>
    public string? On_Server_Exited
    {
        get => _on_server_exited;
        set => _on_server_exited = value;
    }


    /// <summary>
    /// rtp server 超时未收到数据
    /// </summary>
    public string On_Rtp_Server_Timeout
    {
        get => _on_rtp_server_timeout;
        set => _on_rtp_server_timeout = value;
    }

    /// <summary>
    /// hook api最大等待回复时间，单位秒
    /// </summary>
    public int? TimeoutSec
    {
        get => _timeoutSec;
        set => _timeoutSec = value;
    }

    /// <summary>
    /// keepalive hook触发间隔,单位秒，float类型
    /// </summary>
    public float? Alive_Interval
    {
        get => _alive_interval;
        set => _alive_interval = value;
    }

    /// <summary>
    /// hook通知失败重试次数,正整数。为0不重试，1时重试一次，以此类推
    /// </summary>
    public int? Retry
    {
        get => _retry;
        set => _retry = value;
    }

    /// <summary>
    /// hook通知失败重试延时，单位秒，float型
    /// </summary>
    public float? Retry_Delay
    {
        get => _retry_delay;
        set => _retry_delay = value;
    }
}