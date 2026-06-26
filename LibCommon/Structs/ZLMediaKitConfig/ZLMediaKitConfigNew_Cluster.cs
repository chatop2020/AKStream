using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

/// <summary>
/// 设置源站拉流url模板, 格式跟printf类似，第一个%s指定app,第二个%s指定stream_id,
/// 开启集群模式后，on_stream_not_found和on_stream_none_reader hook将无效.
/// 溯源模式支持以下类型:
/// rtmp方式: rtmp://127.0.0.1:1935/%s/%s
/// rtsp方式: rtsp://127.0.0.1:554/%s/%s
/// hls方式: http://127.0.0.1:80/%s/%s/hls.m3u8
/// http-ts方式: http://127.0.0.1:80/%s/%s.live.ts
/// </summary>
[Serializable]
public class ZLMediaKitConfigNew_Cluster
{
    private string? _origin_url;
    private int? _timeout_sec;
    private int? _retry_count;

    /// <summary>
    /// 支持多个源站，不同源站通过分号(;)分隔
    /// </summary>
    public string Origin_Url
    {
        get => _origin_url;
        set => _origin_url = value;
    }

    /// <summary>
    /// 溯源总超时时长，单位秒，float型；假如源站有3个，那么单次溯源超时时间为timeout_sec除以3
    /// 单次溯源超时时间不要超过general.maxStreamWaitMS配置
    /// </summary>
    public int? Timeout_Sec
    {
        get => _timeout_sec;
        set => _timeout_sec = value;
    }

    /// <summary>
    /// 溯源失败尝试次数，-1时永久尝试
    /// </summary>
    public int? Retry_Count
    {
        get => _retry_count;
        set => _retry_count = value;
    }
}