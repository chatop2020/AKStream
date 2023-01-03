using System;

namespace LibCommon.Structs;

/// <summary>
/// 版本号获取结构
/// </summary>
[Serializable]
public class AKStreamVersions
{
    private string _akstreamWebVersion;
    private string _akstreamKeeperVersion;
    private string _zlmbuildDatetime;

    public string AKStreamWebVersion
    {
        get => _akstreamWebVersion;
        set => _akstreamWebVersion = value;
    }

    public string AKStreamKeeperVersion
    {
        get => _akstreamKeeperVersion;
        set => _akstreamKeeperVersion = value;
    }

    public string ZlmBuildDatetime
    {
        get => _zlmbuildDatetime;
        set => _zlmbuildDatetime = value;
    }
}