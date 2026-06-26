using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_Shell
{
    private int? _maxReqSize;
    private ushort? _port;

    /// <summary>
    /// 调试telnet服务器接受最大bufffer大小
    /// </summary>
    public int? MaxReqSize
    {
        get => _maxReqSize;
        set => _maxReqSize = value;
    }

    /// <summary>
    /// 调试telnet服务器监听端口
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }
}