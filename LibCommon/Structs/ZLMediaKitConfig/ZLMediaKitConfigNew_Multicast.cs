using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_Multicast
{
    private string? _addrMax;
    private string? _addrMin;
    private int? _udpTTL;

    /// <summary>
    /// rtp组播截止组播ip地址
    /// </summary>
    public string AddrMax
    {
        get => _addrMax;
        set => _addrMax = value;
    }

    /// <summary>
    /// rtp组播起始组播ip地址
    /// </summary>
    public string AddrMin
    {
        get => _addrMin;
        set => _addrMin = value;
    }

    /// <summary>
    /// 组播udp ttl
    /// </summary>
    public int? UdpTtl
    {
        get => _udpTTL;
        set => _udpTTL = value;
    }
}