using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse;

public class ResToWebHookOnRtspAuth : ResZLMediaKitResponseBase
{
    private bool? _encrypted;
    private string? _passwd;
    private string? _msg;

    /// <summary>
    /// 是否加密过
    /// </summary>
    public bool? Encrypted
    {
        get => _encrypted;
        set => _encrypted = value;
    }

    /// <summary>
    /// password
    /// </summary>
    public string Passwd
    {
        get => _passwd;
        set => _passwd = value;
    }

    /// <summary>
    /// message
    /// </summary>
    public string Msg
    {
        get => _msg;
        set => _msg = value;
    }
}