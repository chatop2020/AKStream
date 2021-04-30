using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibCommon;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public static class NetWorkMacValue
    {
        private static object lockObj = new object();

        private static ProcessHelper SystemInfoProcessHelper =
            new ProcessHelper(p_StdOutputDataReceived, null!, p_Process_Exited!);


        public static NetWorkStat NetWorkStat = new NetWorkStat();

        static NetWorkMacValue()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ProcessHelper tmpProcess = new ProcessHelper(null!, null!, null!);
                tmpProcess.RunProcess("/sbin/route", "-n get default", 1000, out string std, out string err);
                bool isFound = false;
                if (!string.IsNullOrEmpty(std))
                {
                    string[] tmpStrArr = std.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStrArr.Length > 0)
                    {
                        foreach (var tmpStr in tmpStrArr)
                        {
                            if (isFound)
                            {
                                break;
                            }

                            if (!string.IsNullOrEmpty(tmpStr) && tmpStr.Contains("interface:"))
                            {
                                string str = tmpStr.Replace("interface:", "").Trim();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    tmpProcess.RunProcess("/sbin/ifconfig", str, 1000, out string std1,
                                        out string err1);
                                    if (!string.IsNullOrEmpty(std1))
                                    {
                                        string[] tmpStrArr1 = std1.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                                        if (tmpStrArr1.Length > 0)
                                        {
                                            foreach (var str2 in tmpStrArr1)
                                            {
                                                if (!string.IsNullOrEmpty(str2) && str2.Contains("ether"))
                                                {
                                                    string tmpS = str2.Replace("ether", "").Replace(":", "-").ToUpper()
                                                        .Trim();
                                                    if (!string.IsNullOrEmpty(tmpS))
                                                    {
                                                        NetWorkStat.Mac = tmpS;
                                                        isFound = true;
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
                }

                SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-d -u -s 1 -pid 0");
            }
        }

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return NetWorkStat;
            }
        }

        private static void p_Process_Exited(object sender, EventArgs e)
        {
            SystemInfoProcessHelper.RunProcess("/usr/bin/top", "-d -u -s 1 -pid 0");
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Networks: packets:"))
                {
                    bool b = false;
                    string tmpStr = e.Data;
                    tmpStr = tmpStr.Replace("Networks: packets:", "");
                    string[] tmpStrArr = tmpStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStrArr.Length == 2)
                    {
                        ulong tmpRecvBytes = 0;
                        ulong tmpSendBytes = 0;
                        foreach (var str in tmpStrArr)
                        {
                            if (str.ToUpper().Contains("IN"))
                            {
                                string s1 = str.ToUpper();
                                s1 = s1.Replace("IN", "").Trim();
                                s1 = s1.Substring(s1.IndexOf('/') + 1);
                                string s2 = s1.Substring(0, s1.Length - 1);
                                string s3 = s1.Substring(s1.Length - 1);

                                switch (s3)
                                {
                                    case "B":
                                        b = ulong.TryParse(s2, out tmpRecvBytes);
                                        break;
                                    case "K":
                                        b = ulong.TryParse(s2, out tmpRecvBytes);
                                        tmpRecvBytes = tmpRecvBytes * 1024;
                                        break;
                                    case "M":
                                        b = ulong.TryParse(s2, out tmpRecvBytes);
                                        tmpRecvBytes = tmpRecvBytes * 1024 * 1024;
                                        break;
                                }
                            }

                            if (str.ToUpper().Contains("OUT."))
                            {
                                string s1 = str.ToUpper();
                                s1 = s1.Replace("OUT.", "").Trim();
                                s1 = s1.Substring(s1.IndexOf('/') + 1);
                                string s2 = s1.Substring(0, s1.Length - 1);
                                string s3 = s1.Substring(s1.Length - 1);
                                switch (s3)
                                {
                                    case "B":
                                        b = ulong.TryParse(s2, out tmpSendBytes);
                                        break;
                                    case "K":
                                        b = ulong.TryParse(s2, out tmpSendBytes);
                                        tmpSendBytes = tmpSendBytes * 1024;
                                        break;
                                    case "M":
                                        b = ulong.TryParse(s2, out tmpSendBytes);
                                        tmpSendBytes = tmpSendBytes * 1024 * 1024;
                                        break;
                                }
                            }
                        }

                        if (b)
                        {
                            lock (lockObj)
                            {
                                NetWorkStat.TotalRecvBytes += tmpRecvBytes;
                                NetWorkStat.TotalSendBytes += tmpSendBytes;
                                NetWorkStat.CurrentRecvBytes = tmpRecvBytes;
                                NetWorkStat.CurrentSendBytes = tmpSendBytes;
                                NetWorkStat.UpdateTime = DateTime.Now;
                            }
                        }
                    }
                }
            }
        }
    }
}