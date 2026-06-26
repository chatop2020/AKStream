using System;

namespace LibCommon.Structs.Other;

[Serializable]
public class ResPushStreamGetIP
{
    private string? _ipAddress;
    private string? _deviceId;

    /// <summary>
    /// 设备对应的流媒体服务器ip地址
    /// </summary>
    public string? IpAddress
    {
        get => _ipAddress;
        set => _ipAddress = value;
    }

    /// <summary>
    /// sip设备id
    /// </summary>
    public string? DeviceId
    {
        get => _deviceId;
        set => _deviceId = value;
    }
}