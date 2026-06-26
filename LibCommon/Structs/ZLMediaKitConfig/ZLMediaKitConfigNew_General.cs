using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_General
{
    private int? _enableVhost;
    private int? _flowThreshold;
    private int? _maxStreamWaitMS;
    private int? _streamNoneReaderDelayMS;
    private int? _resetWhenRePlay;
    private int? _mergeWriteMS;
    private string? _mediaServerId;
    private int? _wait_track_ready_ms;
    private int? _wait_add_track_ms;
    private int? _unready_frame_cache;
    private int? _check_nvidia_dev;
    private int? _enable_ffmpeg_log;


    /// <summary>
    /// 是否启用虚拟主机
    /// </summary>
    public int? EnableVhost
    {
        get => _enableVhost;
        set => _enableVhost = value;
    }

    /// <summary>
    /// 播放器或推流器在断开后会触发hook.on_flow_report事件(使用多少流量事件)，
    /// flowThreshold参数控制触发hook.on_flow_report事件阈值，使用流量超过该阈值后才触发，单位KB
    /// </summary>
    public int? FlowThreshold
    {
        get => _flowThreshold;
        set => _flowThreshold = value;
    }

    /// <summary>
    /// 播放最多等待时间，单位毫秒
    /// 播放在播放某个流时，如果该流不存在，
    /// ZLMediaKit会最多让播放器等待maxStreamWaitMS毫秒
    /// 如果在这个时间内，该流注册成功，那么会立即返回播放器播放成功
    /// 否则返回播放器未找到该流，该机制的目的是可以先播放再推流
    /// </summary>
    public int? MaxStreamWaitMs
    {
        get => _maxStreamWaitMS;
        set => _maxStreamWaitMS = value;
    }

    /// <summary>
    /// 某个流无人观看时，触发hook.on_stream_none_reader事件的最大等待时间，单位毫秒
    /// 在配合hook.on_stream_none_reader事件时，可以做到无人观看自动停止拉流或停止接收推流
    /// </summary>
    public int? StreamNoneReaderDelayMs
    {
        get => _streamNoneReaderDelayMS;
        set => _streamNoneReaderDelayMS = value;
    }

    /// <summary>
    /// #拉流代理时如果断流再重连成功是否删除前一次的媒体流数据，如果删除将重新开始，
    /// 如果不删除将会接着上一次的数据继续写(录制hls/mp4时会继续在前一个文件后面写)
    /// </summary>
    public int? ResetWhenRePlay
    {
        get => _resetWhenRePlay;
        set => _resetWhenRePlay = value;
    }

    /// <summary>
    /// 合并写缓存大小(单位毫秒)，合并写指服务器缓存一定的数据后才会一次性写入socket，这样能提高性能，但是会提高延时
    /// 开启后会同时关闭TCP_NODELAY并开启MSG_MORE
    /// </summary>
    public int? MergeWriteMs
    {
        get => _mergeWriteMS;
        set => _mergeWriteMS = value;
    }

    /// <summary>
    /// 服务器唯一id，用于触发hook时区别是哪台服务器
    /// </summary>
    public string MediaServerId
    {
        get => _mediaServerId;
        set => _mediaServerId = value;
    }

    /// <summary>
    /// 最多等待未初始化的Track时间，单位毫秒，超时之后会忽略未初始化的Track
    /// </summary>
    public int? Wait_Track_Ready_Ms
    {
        get => _wait_track_ready_ms;
        set => _wait_track_ready_ms = value;
    }

    /// <summary>
    /// #如果流只有单Track，最多等待若干毫秒，超时后未收到其他Track的数据，则认为是单Track
    /// 如果协议元数据有声明特定track数，那么无此等待时间
    /// </summary>
    public int? Wait_Add_Track_Ms
    {
        get => _wait_add_track_ms;
        set => _wait_add_track_ms = value;
    }

    /// <summary>
    /// 如果track未就绪，我们先缓存帧数据，但是有最大个数限制，防止内存溢出
    /// </summary>
    public int? Unready_Frame_Cache
    {
        get => _unready_frame_cache;
        set => _unready_frame_cache = value;
    }

    /// <summary>
    /// 是否检测nvidia设备
    /// </summary>
    public int? Check_Nvidia_Dev
    {
        get => _check_nvidia_dev;
        set => _check_nvidia_dev = value;
    }

    /// <summary>
    /// 是否开启ffmpeg日志
    /// </summary>
    public int? Enable_FFmpeg_Log
    {
        get => _enable_ffmpeg_log;
        set => _enable_ffmpeg_log = value;
    }
}