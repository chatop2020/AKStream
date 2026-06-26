using System;

namespace LibCommon.Structs.Other;

[Serializable]
public class ReqPushStreamGetIP
{
    private string? _deviceId;

    /// <summary>
    /// sip设备id
    /// </summary>
    public string? DeviceId
    {
        get => _deviceId;
        set => _deviceId = value;
    }
}