using System;
using System.Runtime.InteropServices;

namespace LibSystemInfo
{
    public static class CPUWinLoadValue
    {
        private const int SYSTEM_PERFORMANCEINFORMATION = 2;
        private const int SYSTEM_TIMEINFORMATION = 3;
        private const int NO_ERROR = 0;
        public static double CPULOAD = 0f;
        private static long oldIdleTime;
        private static long oldSystemTime;
        private static double processorCount;

        static CPUWinLoadValue()
        {
            processorCount = Environment.ProcessorCount;
            byte[] timeInfo = new byte[32];
            byte[] perfInfo = new byte[312];
            int ret;
            ret = NtQuerySystemInformation(SYSTEM_TIMEINFORMATION, timeInfo, timeInfo.Length, IntPtr.Zero);
            if (ret != NO_ERROR)
            {
                throw new NotSupportedException();
            }

            ret = NtQuerySystemInformation(SYSTEM_PERFORMANCEINFORMATION, perfInfo, perfInfo.Length, IntPtr.Zero);
            if (ret != NO_ERROR)
            {
                throw new NotSupportedException();
            }

            oldIdleTime = BitConverter.ToInt64(perfInfo, 0);
            oldSystemTime = BitConverter.ToInt64(timeInfo, 8);
        }

        [DllImport("ntdll.dll", EntryPoint = "NtQuerySystemInformation")]
        private static extern int NtQuerySystemInformation(int dwInfoType, byte[] lpStructure, int dwSize,
            IntPtr returnLength);

        public static double Refresh()
        {
            var f = Query();
            if (f < 0)
            {
                f = 0;
            }

            if (f > 100)
            {
                f = 100;
            }

            CPULOAD = Math.Round(f, 2);
            return CPULOAD;
        }

        public static double Query()
        {
            byte[] timeInfo = new byte[32];
            byte[] perfInfo = new byte[312];
            double dbIdleTime, dbSystemTime;
            int ret;
            ret = NtQuerySystemInformation(SYSTEM_TIMEINFORMATION, timeInfo, timeInfo.Length, IntPtr.Zero);
            if (ret != NO_ERROR)
                throw new NotSupportedException();
            ret = NtQuerySystemInformation(SYSTEM_PERFORMANCEINFORMATION, perfInfo, perfInfo.Length, IntPtr.Zero);
            if (ret != NO_ERROR)
                throw new NotSupportedException();
            dbIdleTime = BitConverter.ToInt64(perfInfo, 0) - oldIdleTime;
            dbSystemTime = BitConverter.ToInt64(timeInfo, 8) - oldSystemTime;
            if (dbSystemTime != 0)
                dbIdleTime = dbIdleTime / dbSystemTime;
            dbIdleTime = 100.0 - dbIdleTime * 100.0 / processorCount + 0.5;
            oldIdleTime = BitConverter.ToInt64(perfInfo, 0);
            oldSystemTime = BitConverter.ToInt64(timeInfo, 8);
            return dbIdleTime;
        }
    }
}