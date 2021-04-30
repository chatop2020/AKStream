using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibCommon;

namespace LibSystemInfo
{
    static class CPULinuxLoadValue
    {
        public static double CPULOAD = 0f;

        private static ProcessHelper SystemInfoProcessHelper =
            new ProcessHelper(p_StdOutputDataReceived, null!, p_Process_Exited!);

        static CPULinuxLoadValue()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-b -p0");
            }
        }

        private static void p_Process_Exited(object sender, EventArgs e)
        {
            SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-b -p0");
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.ToString().ToUpper().Contains("CPU(S)"))
                {
                    string tmpStr = e.Data;
                    string[] tmpStrArr = tmpStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStrArr.Length > 0)
                    {
                        foreach (var str in tmpStrArr)
                        {
                            if (str.ToLower().Contains("id"))
                            {
                                string s = str.TrimEnd(new[] {'i', 'd'});
                                s = s.Trim();
                                if (double.TryParse(s, out double f))
                                {
                                    CPULOAD = Math.Round(100f - f, 2);
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