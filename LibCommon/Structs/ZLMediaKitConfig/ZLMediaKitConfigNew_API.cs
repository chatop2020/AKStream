using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_API
{
    private int? _apiDebug;
    private string? _secret;
    private string? _snapRoot;
    private string? _defaultSnap;

    /// <summary>
    /// 是否调试http api,启用调试后，会打印每次http请求的内容和回复
    /// </summary>
    public int? ApiDebug
    {
        get => _apiDebug;
        set => _apiDebug = value;
    }

    /// <summary>
    /// 一些比较敏感的http api在访问时需要提供secret，否则无权限调用
    /// 如果是通过127.0.0.1访问,那么可以不提供secret
    /// </summary>
    public string Secret
    {
        get => _secret;
        set => _secret = value;
    }

    /// <summary>
    /// 截图保存路径根目录，截图通过http api(/index/api/getSnap)生成和获取
    /// </summary>
    public string SnapRoot
    {
        get => _snapRoot;
        set => _snapRoot = value;
    }

    /// <summary>
    /// 默认截图图片，在启动FFmpeg截图后但是截图还未生成时，可以返回默认的预设图片
    /// </summary>
    public string DefaultSnap
    {
        get => _defaultSnap;
        set => _defaultSnap = value;
    }
}