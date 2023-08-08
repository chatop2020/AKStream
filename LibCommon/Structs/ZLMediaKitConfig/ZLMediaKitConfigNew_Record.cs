using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_Record
{
    private string? _appName;
    private int? _fastStart;
    private int? _fileBufSize;
    private int? _fileRepeat;
    private int? _sampleMS;

    /// <summary>
    /// mp4录制或mp4点播的应用名，通过限制应用名，可以防止随意点播
    /// 点播的文件必须放置在此文件夹下
    /// </summary>
    public string AppName
    {
        get => _appName;
        set => _appName = value;
    }

    /// <summary>
    /// mp4录制写文件缓存，单位BYTE,调整参数可以提高文件io性能
    /// </summary>
    public int? FileBufSize
    {
        get => _fileBufSize;
        set => _fileBufSize = value;
    }

    /// <summary>
    /// mp4点播每次流化数据量，单位毫秒，
    /// 减少该值可以让点播数据发送量更平滑，增大该值则更节省cpu资源
    /// </summary>
    public int? SampleMs
    {
        get => _sampleMS;
        set => _sampleMS = value;
    }

    /// <summary>
    /// mp4录制完成后是否进行二次关键帧索引写入头部
    /// </summary>
    public int? FastStart
    {
        get => _fastStart;
        set => _fastStart = value;
    }

    /// <summary>
    /// MP4点播(rtsp/rtmp/http-flv/ws-flv)是否循环播放文件
    /// </summary>
    public int? FileRepeat
    {
        get => _fileRepeat;
        set => _fileRepeat = value;
    }
}