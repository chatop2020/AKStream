using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_RTC
{
    private string? _externIP;
    private ushort? _port;
    private string? _preferredCodecA;
    private string? _preferredCodecV;
    private int? _rembBitRate;
    private ushort? _tcpPort;
    private int? _timeoutSec;

    /// <summary>
    /// rtc播放推流、播放超时时间
    /// </summary>
    public int? TimeoutSec
    {
        get => _timeoutSec;
        set => _timeoutSec = value;
    }

    /// <summary>
    /// 本机对rtc客户端的可见ip，作为服务器时一般为公网ip，可有多个，用','分开，当置空时，会自动获取网卡ip
    /// 同时支持环境变量，以$开头，如"$EXTERN_IP"; 请参考：https://github.com/ZLMediaKit/ZLMediaKit/pull/1786
    /// </summary>
    public string ExternIp
    {
        get => _externIP;
        set => _externIP = value;
    }

    /// <summary>
    /// rtc udp服务器监听端口号，所有rtc客户端将通过该端口传输stun/dtls/srtp/srtcp数据，
    /// 该端口是多线程的，同时支持客户端网络切换导致的连接迁移
    /// 需要注意的是，如果服务器在nat内，需要做端口映射时，必须确保外网映射端口跟该端口一致
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// rtc tcp服务器监听端口号，在udp 不通的情况下，会使用tcp传输数据
    /// 该端口是多线程的，同时支持客户端网络切换导致的连接迁移
    /// 需要注意的是，如果服务器在nat内，需要做端口映射时，必须确保外网映射端口跟该端口一致
    /// </summary>
    public ushort? TcpPort
    {
        get => _tcpPort;
        set => _tcpPort = value;
    }

    /// <summary>
    /// 设置remb比特率，非0时关闭twcc并开启remb。该设置在rtc推流时有效，可以控制推流画质
    /// 目前已经实现twcc自动调整码率，关闭remb根据真实网络状况调整码率
    /// </summary>
    public int? RembBitRate
    {
        get => _rembBitRate;
        set => _rembBitRate = value;
    }

    /// <summary>
    /// rtc支持的音频codec类型,在前面的优先级更高
    /// 以下范例为所有支持的音频codec
    /// </summary>
    public string PreferredCodecA
    {
        get => _preferredCodecA;
        set => _preferredCodecA = value;
    }

    /// <summary>
    /// rtc支持的视频codec类型,在前面的优先级更高
    /// 以下范例为所有支持的视频codec
    /// </summary>
    public string PreferredCodecV
    {
        get => _preferredCodecV;
        set => _preferredCodecV = value;
    }
}