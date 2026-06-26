using System.Globalization;
using System.Text.RegularExpressions;

namespace SystemInfoLibrary.Hardware.RAM
{
    internal class LinuxRAMInfo : RAMInfo
    {
        private string _ramInfo;

        private string RAM_Info => string.IsNullOrEmpty(_ramInfo)
            ? (_ramInfo = Utils.GetCommandExecutionOutput("cat", "/proc/meminfo"))
            : _ramInfo;


        public override ulong Total
        {
            get
            {
                var matches = new Regex(@"MemTotal:\s*(\d+)").Matches(RAM_Info);
                return ulong.TryParse(matches[0].Groups[1].Value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value)
                    ? value
                    : 0;
            }
        }

        public override ulong Free
        {
            get
            {
                var matches = new Regex(@"MemFree:\s*(\d+)").Matches(RAM_Info);
                return ulong.TryParse(matches[0].Groups[1].Value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value)
                    ? value
                    : 0;
            }
        }
    }
}