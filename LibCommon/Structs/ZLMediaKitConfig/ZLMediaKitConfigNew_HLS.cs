using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_HLS
{
    private int? _fileBufSize;
    private int? _segDur;
    private int? _segNum;
    private int? _segRetain;
    private int? _broadcastRecordTs;
    private int? _deleteDelaySec;
    private int? _segKeep;


    /// <summary>
    /// hls写文件的buf大小，调整参数可以提高文件io性能
    /// </summary>
    public int? FileBufSize
    {
        get => _fileBufSize;
        set => _fileBufSize = value;
    }

    /// <summary>
    /// hls最大切片时间
    /// </summary>
    public int? SegDur
    {
        get => _segDur;
        set => _segDur = value;
    }

    /// <summary>
    /// m3u8索引中,hls保留切片个数(实际保留切片个数大2~3个)
    /// 如果设置为0，则不删除切片，而是保存为点播
    /// </summary>
    public int? SegNum
    {
        get => _segNum;
        set => _segNum = value;
    }

    /// <summary>
    /// HLS切片从m3u8文件中移除后，继续保留在磁盘上的个数
    /// </summary>
    public int? SegRetain
    {
        get => _segRetain;
        set => _segRetain = value;
    }

    /// <summary>
    /// 是否广播 ts 切片完成通知
    /// </summary>
    public int? BroadcastRecordTs
    {
        get => _broadcastRecordTs;
        set => _broadcastRecordTs = value;
    }

    /// <summary>
    /// 直播hls文件删除延时，单位秒，issue: #913
    /// </summary>
    public int? DeleteDelaySec
    {
        get => _deleteDelaySec;
        set => _deleteDelaySec = value;
    }

    /// <summary>
    /// 是否保留hls文件，此功能部分等效于segNum=0的情况
    /// 不同的是这个保留不会在m3u8文件中体现
    /// 0为不保留，不起作用
    /// 1为保留，则不删除hls文件，如果开启此功能，注意磁盘大小，或者定期手动清理hls文件
    /// </summary>
    public int? SegKeep
    {
        get => _segKeep;
        set => _segKeep = value;
    }
}