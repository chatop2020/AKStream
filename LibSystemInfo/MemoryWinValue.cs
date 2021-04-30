using System;
using System.Runtime.InteropServices;
using LibCommon.Structs;

namespace LibSystemInfo
{
    /// <summary>
    /// 获取windows系统总物理内存使用情况(总量/已使用/未使用)(返回单位KB)
    /// </summary>
    public static class MemoryWinValue
    {
        private static MemoryInfo _memoryInfo = new MemoryInfo();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

        /// <summary>
        /// 获得当前内存使用情况
        /// </summary>
        /// <returns></returns>
        public static MemoryInfo GetMemoryStatus()
        {
            MEMORY_INFO mi = new MEMORY_INFO();
            mi.dwLength = (uint) Marshal.SizeOf(mi);
            GlobalMemoryStatusEx(ref mi);
            _memoryInfo.Free = mi.ullAvailPhys;
            _memoryInfo.Total = mi.ullTotalPhys;
            _memoryInfo.Used = _memoryInfo.Total - _memoryInfo.Free;
            _memoryInfo.FreePercent =
                Math.Round(
                    100f - (double.Parse(_memoryInfo.Free.ToString()) * 100.00 /
                            double.Parse(_memoryInfo.Total.ToString())), 3);
            _memoryInfo.UpdateTime = DateTime.Now;
            return _memoryInfo;
        }

        //定义内存的信息结构
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength; //当前结构体大小
            public uint dwMemoryLoad; //当前内存使用率
            public ulong ullTotalPhys; //总计物理内存大小
            public ulong ullAvailPhys; //可用物理内存大小
            public ulong ullTotalPageFile; //总计交换文件大小
            public ulong ullAvailPageFile; //总计交换文件大小
            public ulong ullTotalVirtual; //总计虚拟内存大小
            public ulong ullAvailVirtual; //可用虚拟内存大小
            public ulong ullAvailExtendedVirtual; //保留 这个值始终为0
        }
    }
}