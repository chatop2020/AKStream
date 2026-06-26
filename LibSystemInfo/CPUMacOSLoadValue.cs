using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibCommon;

namespace LibSystemInfo
{
    public static class CPUMacOSLoadValue
    {
        public static double CPULOAD = 0;

        private static ProcessHelper SystemInfoProcessHelper =
            new ProcessHelper(p_StdOutputDataReceived, null!, p_Process_Exited!);


        static CPUMacOSLoadValue()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-n0");
            }
        }

        private static void p_Process_Exited(object sender, EventArgs e)
        {
            SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-n0");
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.ToUpper().Contains("CPU USAGE"))
                {
                    string tmpStr = e.Data;
                    tmpStr = tmpStr.Replace("CPU usage:", "", StringComparison.Ordinal);
                    string[] tmpStrArr = tmpStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStrArr.Length > 0)
                    {
                        foreach (var tmps in tmpStrArr)
                        {
                            if (tmps.ToLower().Contains("idle"))
                            {
                                string tmps2 = tmps.TrimEnd(new[] { '%', ' ', 'i', 'd', 'l', 'e' });

                                if (double.TryParse(tmps2, out double a))
                                {
                                    CPULOAD = Math.Round(100f - a, 2);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}