using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_RTMP
{
    private int? _handshakeSecond;
    private int? _keepAliveSecond;
    private ushort? _port;
    private ushort? _sslport;
    private int? _directProxy = 1;
    private int? _enhanced = 0;

    /// <summary>
    /// rtmp必须在此时间内完成握手，否则服务器会断开链接，单位秒
    /// </summary>
    public int? HandshakeSecond
    {
        get => _handshakeSecond;
        set => _handshakeSecond = value;
    }

    /// <summary>
    /// rtmp超时时间，如果该时间内未收到客户端的数据，
    /// 或者tcp发送缓存超过这个时间，则会断开连接，单位秒
    /// </summary>
    public int? KeepAliveSecond
    {
        get => _keepAliveSecond;
        set => _keepAliveSecond = value;
    }

    /// <summary>
    /// rtmp服务器监听端口
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// rtmps服务器监听地址
    /// </summary>
    public ushort? Sslport
    {
        get => _sslport;
        set => _sslport = value;
    }

    /// <summary>
    /// rtmp是否直接代理模式
    /// </summary>
    public int? DirectProxy
    {
        get => _directProxy;
        set => _directProxy = value;
    }

    /// <summary>
    /// h265 rtmp打包采用增强型rtmp标准还是国内拓展标准
    /// </summary>
    public int? Enhanced
    {
        get => _enhanced;
        set => _enhanced = value;
    }
}