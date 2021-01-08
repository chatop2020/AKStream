using System;
using System.Threading;
using SystemInfoLibrary.Hardware;
using SystemInfoLibrary.OperatingSystem;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public class SystemInfo : IDisposable
    {
        private static OperatingSystemInfo _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
        private static HardwareInfo _hardwareInfo = _operatingSystemInfo.Hardware;
        private static OperatingSystemType _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
        private static PerformanceInfo _globalSystemInfo = new PerformanceInfo();
        private static object _lockObj = new object();
        private static bool _abort = false;


        public SystemInfo()
        {
            _globalSystemInfo.Architecture = _operatingSystemInfo.Architecture;
            _globalSystemInfo.OsName = _operatingSystemInfo.Name;
            _globalSystemInfo.CpuCores = Environment.ProcessorCount;
            _globalSystemInfo.FrameworkVersion = _operatingSystemInfo.FrameworkVersion.ToString();
            _globalSystemInfo.SystemType = _operatingSystemType.ToString();
            Thread thread = new Thread(GetInfo);
            thread.Start();
        }

        public void Dispose()
        {
            _abort = true;
            _operatingSystemInfo = null!;
            _hardwareInfo = null!;
            _globalSystemInfo = null!;
            _globalSystemInfo = null!;
        }

        ~SystemInfo()
        {
            Dispose(); //释放非托管资源
        }

        private MemoryInfo getMeminfo()
        {
            MemoryInfo tmp = new MemoryInfo();

            tmp.Total = _hardwareInfo.RAM.Total * 1024;
            tmp.Used = _hardwareInfo.RAM.Total * 1024 - _hardwareInfo.RAM.Free * 1024;
            tmp.Free = _hardwareInfo.RAM.Free * 1024;
            tmp.FreePercent =
                Math.Round(100f - (double.Parse(tmp.Used.ToString()) * 100.00 / double.Parse(tmp.Total.ToString())), 3);
            tmp.UpdateTime = DateTime.Now;
            return tmp;
        }

        private void GetInfo()
        {
            ushort i = 0;
            ushort j = 0;
            while (true)
            {
                if (_abort)
                {
                    break;
                }

                i++;
                j++;
                if (ushort.MaxValue - i < 100)
                {
                    i = 0;
                }

                if (ushort.MaxValue - j < 100)
                {
                    j = 0;
                }

                lock (_lockObj)
                {
                    if ((j % 10 == 0 || j == 1) && _operatingSystemType != OperatingSystemType.Windows) //10秒更新一次内存情况
                    {
                        _operatingSystemInfo = null!;
                        _hardwareInfo = null!;
                        _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
                        _hardwareInfo = _operatingSystemInfo.Hardware;
                        _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
                        _globalSystemInfo.MemoryInfo = getMeminfo();
                    }

                    if (i % 120 == 0 || i == 1) //2分钟更新一次硬盘情况
                    {
                        _globalSystemInfo.DriveInfo = DiskInfoValue.GetDrivesInfo();
                    }

                    switch (_operatingSystemType)
                    {
                        case OperatingSystemType.Windows:
                            _globalSystemInfo.CpuLoad = CPUWinLoadValue.CPULOAD;
                            /*
                             * 由于.net core下无法正确获取到windows的流量信息，这边将Windows系统的网络参数略过
                             * 在Windows平台上，需要自己把各配置文件写好，系统自动生成可能会存在问题
                             */
                            //_globalSystemInfo.NetWorkStat = NetWorkWinValue.GetNetworkStat();
                            _globalSystemInfo.NetWorkStat = new NetWorkStat();
                            _globalSystemInfo.NetWorkStat.Mac = "00:00:00:00:00";
                            _globalSystemInfo.NetWorkStat.UpdateTime = DateTime.Now;
                            _globalSystemInfo.NetWorkStat.CurrentRecvBytes = 0;
                            _globalSystemInfo.NetWorkStat.CurrentSendBytes = 0;
                            _globalSystemInfo.NetWorkStat.TotalRecvBytes = 0;
                            _globalSystemInfo.NetWorkStat.TotalSendBytes = 0;


                            if ((j % 10 == 0 || j == 1) &&
                                _operatingSystemType == OperatingSystemType.Windows) //10秒更新一次内存情况
                            {
                                _globalSystemInfo.MemoryInfo = MemoryWinValue.GetMemoryStatus();
                            }

                            break;
                        case OperatingSystemType.MacOSX:
                            _globalSystemInfo.CpuLoad = CPUMacOSLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkMacValue.GetNetworkStat();
                            break;
                        case OperatingSystemType.Unix:
                            _globalSystemInfo.CpuLoad = CPULinuxLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkLinuxValue.GetNetworkStat();
                            break;
                    }

                    _globalSystemInfo.UpdateTime = DateTime.Now;
                }

                Thread.Sleep(1000);
            }
        }


        public string GetSystemInfoJson()
        {
            lock (_lockObj)
            {
                return JsonHelper.ToJson(_globalSystemInfo);
            }
        }

        public PerformanceInfo GetSystemInfoObject()
        {
            lock (_lockObj)
            {
                return _globalSystemInfo;
            }
        }
    }
}