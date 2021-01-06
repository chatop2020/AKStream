using System;

namespace LibCommon.Enums
{
    /// <summary>
    /// 设备的流接入类型
    /// </summary>
    [Serializable]
    public enum DeviceStreamType
    {
        GB28181,
        Rtsp,
        Http,
        Rtmp,
    }
}