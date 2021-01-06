using System.Text.RegularExpressions;

namespace LibCommon.Structs.GB28181.Sys.Net
{
    public static class IPSocketUtils
    {
        public static bool IsIPSocket(string socket)
        {
            if (socket == null || socket.Trim().Length == 0)
            {
                return false;
            }
            else
            {
                return Regex.Match(socket, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}(:\d{1,5})$", RegexOptions.Compiled)
                    .Success;
            }
        }
    }
}