using System;
using System.Threading;
using LibCommon.Structs;
using LibSystemInfo;
using Newtonsoft.Json;

namespace Test_LibSystemInfo
{
    class Program
    {
        public static PerformanceInfo KeeperPerformanceInfo = new PerformanceInfo();
        private static SystemInfo _keeperSystemInfo = new SystemInfo();
        static void Main(string[] args)
        {
        
            while (true)
            {
                KeeperPerformanceInfo = _keeperSystemInfo.GetSystemInfoObject();
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.Architecture,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.CpuCores,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.CpuLoad,Formatting.Indented));
                Console.WriteLine( LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.DriveInfo,Formatting.Indented));
                Console.WriteLine( LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.FrameworkVersion,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.MemoryInfo,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.OsName,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.SystemType,Formatting.Indented));
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.UpdateTime,Formatting.Indented));
               
                Console.WriteLine(LibCommon.JsonHelper.ToJson(KeeperPerformanceInfo.NetWorkStat,Formatting.Indented));
                Thread.Sleep(1000);
            }  
            Console.WriteLine("Hello World!");
        }
    }
}