using System.Globalization;
using System.Text.RegularExpressions;

namespace SystemInfoLibrary.Hardware.RAM
{
    internal class BSDRAMInfo : RAMInfo
    {
        private string _vmStats;

        private string VMStats => string.IsNullOrEmpty(_vmStats)
            ? (_vmStats = Utils.GetCommandExecutionOutput("vm_stat", ""))
            : _vmStats;


        public override ulong Total => (ulong) Utils.GetSysCtlPropertyInt64("hw.memsize") / 1024;

        public override ulong Free
        {
            get
            {
                var matches = new Regex(@"Pages free:\s*(\d+)").Matches(VMStats);
                return ulong.TryParse(matches[0].Groups[1].Value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value)
                    ? value * (ulong) Utils.GetPageSize() / 1024
                    : 0;
            }
        }
    }
}