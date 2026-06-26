using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

/// <summary>
/// FFmpeg拉流再推流的命令模板，通过该模板可以设置再编码的一些参数
/// </summary>
[Serializable]
public class ZLMediaKitConfigNew_FFMPEG
{
    private string? _bin;
    private string? _cmd;
    private string? _snap;
    private string? _log;
    private int? _restart_sec;

    /// <summary>
    /// ffmpeg可执行文件路径
    /// </summary>
    public string Bin
    {
        get => _bin;
        set => _bin = value;
    }

    /// <summary>
    /// 命令模板
    /// </summary>
    public string Cmd
    {
        get => _cmd;
        set => _cmd = value;
    }

    /// <summary>
    /// Fmpeg生成截图的命令，可以通过修改该配置改变截图分辨率或质量
    /// </summary>
    public string Snap
    {
        get => _snap;
        set => _snap = value;
    }

    /// <summary>
    /// FFmpeg日志的路径，如果置空则不生成FFmpeg日志
    /// </summary>
    public string Log
    {
        get => _log;
        set => _log = value;
    }

    /// <summary>
    /// 自动重启的时间(秒), 默认为0, 也就是不自动重启. 主要是为了避免长时间ffmpeg拉流导致的不同步现象
    /// </summary>
    public int? Restart_Sec
    {
        get => _restart_sec;
        set => _restart_sec = value;
    }
}