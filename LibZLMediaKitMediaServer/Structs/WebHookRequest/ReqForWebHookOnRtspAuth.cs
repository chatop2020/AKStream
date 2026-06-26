using System;

namespace LibZLMediaKitMediaServer.Structs.WebHookRequest;

[Serializable]
public class ReqForWebHookOnRtspAuth
{
    private string? _mediaServerId;
    private string? _app;
    private string? _id;
    private string? _ip;
    private string? _must_no_encrypt;
    private string? _params;
    private ushort? _port;
    private string? _realm;
    private string? _schema;
    private string? _stream;
    private string? _user_name;
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
    /// rtsp播放器ip
    /// </summary>
    public string Ip
    {
        get => _ip;
        set => _ip = value;
    }

    /// <summary>
    /// 请求的密码是否必须为明文(base64鉴权需要明文密码)
    /// </summary>
    public string Must_No_Encrypt
    {
        get => _must_no_encrypt;
        set => _must_no_encrypt = value;
    }

    /// <summary>
    /// rtsp url参数
    /// </summary>
    public string Params
    {
        get => _params;
        set => _params = value;
    }

    /// <summary>
    /// 端口
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// rtsp播放鉴权加密realm
    /// </summary>
    public string Realm
    {
        get => _realm;
        set => _realm = value;
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
    /// 用户名
    /// </summary>
    public string User_Name
    {
        get => _user_name;
        set => _user_name = value;
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