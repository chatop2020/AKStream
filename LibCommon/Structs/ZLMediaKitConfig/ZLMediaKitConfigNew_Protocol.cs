using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

/// <summary>
/// 转协议相关开关；如果addStreamProxy api和on_publish hook回复未指定转协议参数，则采用这些配置项
/// </summary>
[Serializable]
public class ZLMediaKitConfigNew_Protocol
{
    private int? _add_mute_audio;
    private int? _auto_close;
    private int? _continue_push_ms;
    private int? _enable_audio;
    private int? _enable_fmp4;
    private int? _enable_hls;
    private int? _enable_hls_fmp4;
    private int? _enable_mp4;
    private int? _enable_rtmp;
    private int? _enable_rtsp;
    private int? _enable_ts;
    private int? _fmp4_demand;
    private int? _hls_demand;
    private string? _hls_save_path;
    private int? _modify_stamp;
    private int? _mp4_as_player;
    private int? _mp4_max_second;
    private string? _mp4_save_path;
    private int? _rtmp_demand;
    private int? _rtsp_demand;
    private int? _ts_demand;

    /// <summary>
    /// 转协议时，是否开启帧级时间戳覆盖
    /// </summary>
    public int? Modify_Stamp
    {
        get => _modify_stamp;
        set => _modify_stamp = value;
    }

    /// <summary>
    /// 转协议是否开启音频
    /// </summary>
    public int? Enable_Audio
    {
        get => _enable_audio;
        set => _enable_audio = value;
    }

    /// <summary>
    /// 添加acc静音音频，在关闭音频时，此开关无效
    /// </summary>
    public int? Add_Mute_Audio
    {
        get => _add_mute_audio;
        set => _add_mute_audio = value;
    }

    /// <summary>
    /// 无人观看时，是否直接关闭(而不是通过on_none_reader hook返回close)
    /// 此配置置1时，此流如果无人观看，将不触发on_none_reader hook回调，
    /// 而是将直接关闭流
    /// </summary>
    /// <value></value>
    public int? Auto_Close
    {
        get => _auto_close;
        set => _auto_close = value;
    }


    /// <summary>
    /// 推流断开后可以在超时时间内重新连接上继续推流，这样播放器会接着播放。
    /// 置0关闭此特性(推流断开会导致立即断开播放器)
    /// 此参数不应大于播放器超时时间;单位毫秒
    /// </summary>
    public int? Continue_Push_Ms
    {
        get => _continue_push_ms;
        set => _continue_push_ms = value;
    }

    /// <summary>
    /// 是否开启转换为hls
    /// </summary>
    public int? Enable_Hls
    {
        get => _enable_hls;
        set => _enable_hls = value;
    }

    /// <summary>
    /// 是否开启转换为hls(fmp4)
    /// </summary>
    /// <value></value>
    public int? Enable_Hls_Fmp4
    {
        get => _enable_hls_fmp4;
        set => _enable_hls_fmp4 = value;
    }

    /// <summary>
    /// 是否开启MP4录制
    /// </summary>
    public int? Enable_Mp4
    {
        get => _enable_mp4;
        set => _enable_mp4 = value;
    }

    /// <summary>
    /// 是否开启转换为rtsp/webrtc
    /// </summary>
    public int? Enable_Rtsp
    {
        get => _enable_rtsp;
        set => _enable_rtsp = value;
    }

    /// <summary>
    /// 是否开启转换为rtmp/flv
    /// </summary>
    public int? Enable_Rtmp
    {
        get => _enable_rtmp;
        set => _enable_rtmp = value;
    }

    /// <summary>
    /// 是否开启转换为http-ts/ws-ts
    /// </summary>
    public int? Enable_Ts
    {
        get => _enable_ts;
        set => _enable_ts = value;
    }

    /// <summary>
    /// 是否开启转换为http-fmp4/ws-fmp4
    /// </summary>
    public int? Enable_Fmp4
    {
        get => _enable_fmp4;
        set => _enable_fmp4 = value;
    }

    /// <summary>
    /// 是否将mp4录制当做观看者
    /// </summary>
    public int? Mp4_As_Player
    {
        get => _mp4_as_player;
        set => _mp4_as_player = value;
    }

    /// <summary>
    /// mp4切片大小，单位秒
    /// </summary>
    public int? Mp4_Max_Second
    {
        get => _mp4_max_second;
        set => _mp4_max_second = value;
    }

    /// <summary>
    /// mp4录制保存路径
    /// </summary>
    public string Mp4_Save_Path
    {
        get => _mp4_save_path;
        set => _mp4_save_path = value;
    }

    /// <summary>
    /// hls录制保存路径
    /// </summary>
    public string Hls_Save_Path
    {
        get => _hls_save_path;
        set => _hls_save_path = value;
    }

    /*
    以下是按需转协议的开关，在测试ZLMediaKit的接收推流性能时，请把下面开关置1
    如果某种协议你用不到，你可以把以下开关置1以便节省资源(但是还是可以播放，只是第一个播放者体验稍微差点)，
     如果某种协议你想获取最好的用户体验，请置0(第一个播放者可以秒开，且不花屏)
    */

    /// <summary>
    /// hls协议是否按需生成，如果hls.segNum配置为0(意味着hls录制)，那么hls将一直生成(不管此开关)
    /// </summary>
    public int? Hls_Demand
    {
        get => _hls_demand;
        set => _hls_demand = value;
    }

    /// <summary>
    /// rtsp[s]协议是否按需生成
    /// </summary>
    public int? Rtsp_Demand
    {
        get => _rtsp_demand;
        set => _rtsp_demand = value;
    }

    /// <summary>
    /// rtmp[s]、http[s]-flv、ws[s]-flv协议是否按需生成
    /// </summary>
    public int? Rtmp_Demand
    {
        get => _rtmp_demand;
        set => _rtmp_demand = value;
    }

    /// <summary>
    /// http[s]-ts协议是否按需生成
    /// </summary>
    public int? Ts_Demand
    {
        get => _ts_demand;
        set => _ts_demand = value;
    }

    /// <summary>
    /// http[s]-fmp4、ws[s]-fmp4协议是否按需生成
    /// </summary>
    public int? Fmp4_Demand
    {
        get => _fmp4_demand;
        set => _fmp4_demand = value;
    }
}