using System.Globalization;
using System.Text.RegularExpressions;

namespace SystemInfoLibrary.Hardware.CPU
{
    internal class LinuxCPUInfo : CPUInfo
    {
        private readonly string _cpuInfo;


        public LinuxCPUInfo(string cpuInfo)
        {
            _cpuInfo = cpuInfo;
        }

        public override string Name
        {
            get
            {
                var matches = new Regex(@"model name\s*:\s*(.*)").Matches(_cpuInfo);
                var value = matches[0].Groups[1].Value;
                return string.IsNullOrEmpty(value) ? "Unknown" : value;
            }
        }

        public override string Brand
        {
            get
            {
                var matches = new Regex(@"vendor_id\s*:\s*(.*)").Matches(_cpuInfo);
                var value = matches[0].Groups[1].Value;
                return string.IsNullOrEmpty(value) ? "Unknown" : value;
            }
        }

        public override string Architecture
        {
            get
            {
                var matches = new Regex(@"flags\s*:(.*)").Matches(_cpuInfo);
                var value = matches[0].Groups[1].Value;
                if (!string.IsNullOrEmpty(value))
                    if (value.Contains(" lm") || value.Contains(" x86-64"))
                        return "x64";
                return "x86";
            }
        }

        public override int PhysicalCores
        {
            get
            {
                var matches = new Regex(@"cpu cores\s*:\s*(\d*)").Matches(_cpuInfo);
                return int.TryParse(matches[0].Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture,
                    out var value)
                    ? value
                    : 0;
            }
        }

        public override int LogicalCores
        {
            get
            {
                var matches = new Regex(@"siblings\s*:\s*(\d*)").Matches(_cpuInfo);
                return int.TryParse(matches[0].Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture,
                    out var value)
                    ? value
                    : 0;
            }
        }

        public override double Frequency
        {
            get
            {
                var matches = new Regex(@"cpu MHz\s*:\s*([0-9]*(?:\.[0-9]+)?)").Matches(_cpuInfo);
                return double.TryParse(matches[0].Groups[1].Value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value)
                    ? value
                    : 0;
            }
        }
    }
}