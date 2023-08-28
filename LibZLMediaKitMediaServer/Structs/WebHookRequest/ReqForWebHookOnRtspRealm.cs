namespace LibZLMediaKitMediaServer.Structs.WebHookRequest;

public class ReqForWebHookOnRtspRealm
{
    private string? _mediaServerId;
    private string? _app;
    private string? _id;
    private string? _params;
    private ushort? _port;
    private string? _schema;
    private string? _stream;
    private string? _vhost;

    /// <summary>
    /// 流媒体服务器id
    /// </summary>
    public string MediaServerId
    {
        get => _mediaServerId;
        set => _mediaServerId = value;
    }

    /// <summary>
    /// app
    /// </summary>
    public string App
    {
        get => _app;
        set => _app = value;
    }

    /// <summary>
    /// TCP链接唯一ID
    /// </summary>
    public string Id
    {
        get => _id;
        set => _id = value;
    }

    /// <summary>
    /// 播放rtsp url参数
    /// </summary>
    public string Params
    {
        get => _params;
        set => _params = value;
    }

    /// <summary>
    /// rtsp播放器端口号
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// rtsp或rtsps
    /// </summary>
    public string Schema
    {
        get => _schema;
        set => _schema = value;
    }

    /// <summary>
    /// streamid
    /// </summary>
    public string Stream
    {
        get => _stream;
        set => _stream = value;
    }

    /// <summary>
    /// vhost
    /// </summary>
    public string Vhost
    {
        get => _vhost;
        set => _vhost = value;
    }
}