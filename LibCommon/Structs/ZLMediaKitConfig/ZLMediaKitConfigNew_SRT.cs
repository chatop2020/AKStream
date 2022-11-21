using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_SRT
{
    private int? _timeoutSec;
    private ushort? _port;
    private int? _latencyMul;
    private int? _pktBufSize;

    /// <summary>
    /// srt播放推流、播放超时时间,单位秒
    /// </summary>
    public int? TimeoutSec
    {
        get => _timeoutSec;
        set => _timeoutSec = value;
    }

    /// <summary>
    /// srt udp服务器监听端口号，所有srt客户端将通过该端口传输srt数据，
    /// 该端口是多线程的，同时支持客户端网络切换导致的连接迁移
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// srt 协议中延迟缓存的估算参数，在握手阶段估算rtt ,然后latencyMul*rtt 为最大缓存时长，此参数越大，表示等待重传的时长就越大
    /// </summary>
    public int? LatencyMul
    {
        get => _latencyMul;
        set => _latencyMul = value;
    }

    /// <summary>
    /// 包缓存的大小
    /// </summary>
    public int? PktBufSize
    {
        get => _pktBufSize;
        set => _pktBufSize = value;
    }
}