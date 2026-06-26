using LibZLMediaKitMediaServer.Structs.WebResponse.ZLMediaKit;

namespace LibZLMediaKitMediaServer.Structs.WebHookResponse;

public class ResToWebHookOnRtspRealm : ResZLMediaKitResponseBase
{
    private string? _realm;

    public string Realm
    {
        get => _realm;
        set => _realm = value;
    }
}