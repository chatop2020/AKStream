using System;
using System.Diagnostics;
using System.IO;
using LibCommon;
using LibCommon.Structs;

namespace LibSystemInfo
{
    /// <summary>
    /// 由于.net core在非托管的内存读写上存在bug,无法运行，因此此模块在Fw4.0上实现了核心代码，并由Fw4.0编译了exe文件供
    /// 模块调用，因此在使用此模块时，必须要包含WinNetworkStaCli.exe可执行文件
    /// </summary>
    public static class NetWorkWinValue
    {
        private static object lockObj = new object();
        private static string binPath = GCommon.BaseStartPath + "/WinNetworkStaCli.exe";

        private static ProcessHelper SystemInfoProcessHelper =
            new ProcessHelper(p_StdOutputDataReceived, null!, p_Process_Exited!);

        private static ulong _perSendBytes = 0;
        private static ulong _perRecvBytes = 0;

        public static NetWorkStat NetWorkStat = new NetWorkStat();


        
        
        static NetWorkWinValue()
        {
            if (File.Exists(binPath))
            {
                //Windows下，父亲进程退出后，子进程没有被退出
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(binPath));
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                    {
                        if (process.HasExited == false)
                        {
                            process.Kill();
                        }
                    }
                }

                SystemInfoProcessHelper.RunProcess(binPath, "");
            }
            else
            {
                throw new FileNotFoundException(binPath + " not found.");
            }
        }

        private static void p_Process_Exited(object sender, EventArgs e)
        {
            SystemInfoProcessHelper.RunProcess(binPath, "");
            
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ulong tmpSendBytes = 0;
                ulong tmpRecvBytes = 0;
                string[] tmpStrArr = e.Data.Trim().Split("]-[", StringSplitOptions.RemoveEmptyEntries);
                if (tmpStrArr.Length == 3)
                {
                    foreach (var tmpStr in tmpStrArr)
                    {
                        if (tmpStr.Contains("MAC"))
                        {
                            NetWorkStat.Mac = tmpStr.Replace("MAC:", "").Trim();
                        }

                        if (tmpStr.Contains("发送"))
                        {
                            var per = tmpStr.Replace("发送:", "");
                            if (!ulong.TryParse(per.Trim(), out tmpSendBytes))
                            {
                                break;
                            }
                        }

                        if (tmpStr.Contains("接收"))
                        {
                            var per = tmpStr.Replace("接收:", "");
                            if (!ulong.TryParse(per.Trim(), out tmpRecvBytes))
                            {
                                break;
                            }
                        }
                    }

                    if (tmpSendBytes > 0 && tmpRecvBytes > 0)
                    {
                        if (_perSendBytes == 0 && _perRecvBytes == 0) //第一次
                        {
                            lock (lockObj)
                            {
                                _perSendBytes = tmpSendBytes;
                                _perRecvBytes = tmpRecvBytes;
                                NetWorkStat.CurrentRecvBytes = 0;
                                NetWorkStat.CurrentSendBytes = 0;
                                NetWorkStat.TotalRecvBytes = 0;
                                NetWorkStat.TotalSendBytes = 0;
                                NetWorkStat.UpdateTime = DateTime.Now;
                            }
                        }
                        else //有数据以后，每次计算差值
                        {
                            lock (lockObj)
                            {
                                ulong subSendBytes = tmpSendBytes - _perSendBytes;
                                ulong subRecvBytes = tmpRecvBytes - _perRecvBytes;
                                _perSendBytes = tmpSendBytes;
                                _perRecvBytes = tmpRecvBytes;
                                NetWorkStat.CurrentRecvBytes = subRecvBytes;
                                NetWorkStat.CurrentSendBytes = subSendBytes;
                                NetWorkStat.TotalRecvBytes += subRecvBytes;
                                NetWorkStat.TotalSendBytes += subSendBytes;
                                NetWorkStat.UpdateTime = DateTime.Now;
                            }
                        }
                    }
                }
            }
        }

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return NetWorkStat;
            }
        }
    }
}