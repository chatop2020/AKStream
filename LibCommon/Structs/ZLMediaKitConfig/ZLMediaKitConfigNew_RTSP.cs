using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_RTSP
{
    private int? _authBasic;
    private int? _directProxy;
    private int? _handshakeSecond;
    private int? _keepAliveSecond;
    private int? _lowLatency;
    private ushort? _port;
    private int? _rtpTransportType;
    private ushort? _sslport;


    /// <summary>
    /// rtsp专有鉴权方式是采用base64还是md5方式
    /// </summary>
    public int? AuthBasic
    {
        get => _authBasic;
        set => _authBasic = value;
    }

    /// <summary>
    /// rtsp拉流、推流代理是否是直接代理模式
    /// 直接代理后支持任意编码格式，但是会导致GOP缓存无法定位到I帧，可能会导致开播花屏
    /// 并且如果是tcp方式拉流，如果rtp大于mtu会导致无法使用udp方式代理
    /// 假定您的拉流源地址不是264或265或AAC，那么你可以使用直接代理的方式来支持rtsp代理
    /// 如果你是rtsp推拉流，但是webrtc播放，也建议关闭直接代理模式，
    /// 因为直接代理时，rtp中可能没有sps pps,会导致webrtc无法播放; 另外webrtc也不支持Single NAL Unit Packets类型rtp
    /// 默认开启rtsp直接代理，rtmp由于没有这些问题，是强制开启直接代理的
    /// </summary>
    public int? DirectProxy
    {
        get => _directProxy;
        set => _directProxy = value;
    }

    /// <summary>
    /// rtsp必须在此时间内完成握手，否则服务器会断开链接，单位秒
    /// </summary>
    public int? HandshakeSecond
    {
        get => _handshakeSecond;
        set => _handshakeSecond = value;
    }

    /// <summary>
    /// rtsp超时时间，如果该时间内未收到客户端的数据，
    /// 或者tcp发送缓存超过这个时间，则会断开连接，单位秒
    /// </summary>
    public int? KeepAliveSecond
    {
        get => _keepAliveSecond;
        set => _keepAliveSecond = value;
    }

    /// <summary>
    /// rtsp服务器监听地址
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// rtsps服务器监听地址
    /// </summary>
    public ushort? Sslport
    {
        get => _sslport;
        set => _sslport = value;
    }

    /// <summary>
    /// rtsp 转发是否使用低延迟模式，当开启时，不会缓存rtp包，来提高并发，可以降低一帧的延迟
    /// </summary>
    public int? LowLatency
    {
        get => _lowLatency;
        set => _lowLatency = value;
    }

    /// <summary>
    /// 强制协商rtp传输方式 (0:TCP,1:UDP,2:MULTICAST,-1:不限制)
    /// 当客户端发起RTSP SETUP的时候如果传输类型和此配置不一致则返回461 Unsupported transport
    /// 迫使客户端重新SETUP并切换到对应协议。目前支持FFMPEG和VLC
    /// </summary>
    /// <value></value>
    public int? RtpTransportType
    {
        get => _rtpTransportType;
        set => _rtpTransportType = value;
    }
}