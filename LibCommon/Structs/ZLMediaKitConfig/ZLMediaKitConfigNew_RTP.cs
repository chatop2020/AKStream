using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_RTP
{
    private int? _audioMtuSize;
    private int? _h264_stap_a;
    private int? _lowLatency;
    private int? _rtpMaxSize;
    private int? _videoMtuSize;


    /// <summary>
    /// 音频mtu大小，该参数限制rtp最大字节数，推荐不要超过1400
    /// 加大该值会明显增加直播延时
    /// </summary>
    public int? AudioMtuSize
    {
        get => _audioMtuSize;
        set => _audioMtuSize = value;
    }

    /// <summary>
    /// 视频mtu大小，该参数限制rtp最大字节数，推荐不要超过1400
    /// </summary>
    public int? VideoMtuSize
    {
        get => _videoMtuSize;
        set => _videoMtuSize = value;
    }

    /// <summary>
    /// rtp包最大长度限制，单位KB,主要用于识别TCP上下文破坏时，获取到错误的rtp
    /// </summary>
    public int? RtpMaxSize
    {
        get => _rtpMaxSize;
        set => _rtpMaxSize = value;
    }

    /// <summary>
    ///  rtp 打包时，低延迟开关，默认关闭（为0），h264存在一帧多个slice（NAL）的情况，在这种情况下，如果开启可能会导致画面花屏
    /// </summary>
    public int? LowLatency
    {
        get => _lowLatency;
        set => _lowLatency = value;
    }

    /// <summary>
    /// H264 rtp打包模式是否采用stap-a模式(为了在老版本浏览器上兼容webrtc)还是采用Single NAL unit packet per H.264 模式
    /// 有些老的rtsp设备不支持stap-a rtp，设置此配置为0可提高兼容性
    /// </summary>
    /// <value></value>
    public int? H264_Stap_A
    {
        get => _h264_stap_a;
        set => _h264_stap_a = value;
    }
}