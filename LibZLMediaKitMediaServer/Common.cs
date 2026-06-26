namespace LibZLMediaKitMediaServer;

public static class Common
{
    private static string _loggerHead = "LibZlMediaKitMediaServer";

    public static string LoggerHead
    {
        get => _loggerHead;
        set => _loggerHead = value;
    }
}