using System;

namespace LibCommon.Structs.ZLMediaKitConfig;

[Serializable]
public class ZLMediaKitConfigNew_Http
{
    private string? _allow_cross_domains;
    private string? _allow_ip_range;
    private string? _charSet;
    private int? _dirMenu;
    private string? _forbidCacheSuffix;
    private string? _forwarded_ip_header;
    private int? _keepAliveSecond;
    private int? _maxReqSize;
    private string? _notFound;
    private ushort? _port;
    private string? _rootPath;
    private int? _sendBufSize;
    private ushort? _sslport;
    private string? _virtualPath;


    /// <summary>
    /// http服务器字符编码，windows上默认gb2312
    /// </summary>
    public string CharSet
    {
        get => _charSet;
        set => _charSet = value;
    }

    /// <summary>
    /// http链接超时时间
    /// </summary>
    public int? KeepAliveSecond
    {
        get => _keepAliveSecond;
        set => _keepAliveSecond = value;
    }

    /// <summary>
    /// http请求体最大字节数，如果post的body太大，则不适合缓存body在内存
    /// </summary>
    public int? MaxReqSize
    {
        get => _maxReqSize;
        set => _maxReqSize = value;
    }

    /// <summary>
    /// 404网页内容，用户可以自定义404网页
    /// </summary>
    public string NotFound
    {
        get => _notFound;
        set => _notFound = value;
    }

    /// <summary>
    /// http端口
    /// </summary>
    public ushort? Port
    {
        get => _port;
        set => _port = value;
    }

    /// <summary>
    /// http文件服务器根目录
    /// 可以为相对(相对于本可执行程序目录)或绝对路径
    /// </summary>
    public string RootPath
    {
        get => _rootPath;
        set => _rootPath = value;
    }

    /// <summary>
    /// http文件服务器读文件缓存大小，单位BYTE，调整该参数可以优化文件io性能
    /// </summary>
    public int? SendBufSize
    {
        get => _sendBufSize;
        set => _sendBufSize = value;
    }

    /// <summary>
    /// https服务器监听端口
    /// </summary>
    public ushort? SSLport
    {
        get => _sslport;
        set => _sslport = value;
    }

    /// <summary>
    /// 是否显示文件夹菜单，开启后可以浏览文件夹
    /// </summary>
    public int? DirMenu
    {
        get => _dirMenu;
        set => _dirMenu = value;
    }

    /// <summary>
    /// 虚拟目录, 虚拟目录名和文件路径使用","隔开，多个配置路径间用";"隔开
    /// 例如赋值为 app_a,/path/to/a;app_b,/path/to/b 那么
    /// 访问 http://127.0.0.1/app_a/file_a 对应的文件路径为 /path/to/a/file_a
    /// 访问 http://127.0.0.1/app_b/file_b 对应的文件路径为 /path/to/b/file_b
    /// 访问其他http路径,对应的文件路径还是在rootPath内
    /// </summary>
    public string VirtualPath
    {
        get => _virtualPath;
        set => _virtualPath = value;
    }

    /// <summary>
    /// 禁止后缀的文件使用mmap缓存，使用“,”隔开
    /// 例如赋值为 .mp4,.flv
    /// 那么访问后缀为.mp4与.flv 的文件不缓存
    /// </summary>
    public string ForbidCacheSuffix
    {
        get => _forbidCacheSuffix;
        set => _forbidCacheSuffix = value;
    }

    /// <summary>
    /// 可以把http代理前真实客户端ip放在http头中：https://github.com/ZLMediaKit/ZLMediaKit/issues/1388
    /// 切勿暴露此key，否则可能导致伪造客户端ip
    /// </summary>
    public string Forwarded_Ip_Header
    {
        get => _forwarded_ip_header;
        set => _forwarded_ip_header = value;
    }

    /// <summary>
    /// 默认允许所有跨域请求
    /// </summary>
    public string? Allow_Cross_Domains
    {
        get => _allow_cross_domains;
        set => _allow_cross_domains = value;
    }

    /// <summary>
    /// 允许访问http api和http文件索引的ip地址范围白名单，置空情况下不做限制
    /// </summary>
    public string? Allow_Ip_Range
    {
        get => _allow_ip_range;
        set => _allow_ip_range = value;
    }
}