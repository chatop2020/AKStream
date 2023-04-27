using System;
using System.IO;
using System.Threading;
using LibCommon;
using LibCommon.Structs;
using LibSystemInfo;
using Newtonsoft.Json;
using JsonHelper = LibCommon.JsonHelper;

namespace Test_LibSystemInfo
{
    class Program
    {
        public static PerformanceInfo KeeperPerformanceInfo = new PerformanceInfo();
        private static SystemInfo _keeperSystemInfo = new SystemInfo();

        static void Main(string[] args)
        {


           Console.WriteLine(UtilsHelper.DirAreMounttedAndWriteableForLinux("/home/disk/record/record/rtsp/E56A7514"));
          
            
            while (true)
            {
                KeeperPerformanceInfo = _keeperSystemInfo.GetSystemInfoObject();
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.Architecture, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.CpuCores, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.CpuLoad, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.DriveInfo, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.FrameworkVersion, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.MemoryInfo, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.OsName, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.SystemType, Formatting.Indented));
                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.UpdateTime, Formatting.Indented));

                Console.WriteLine(JsonHelper.ToJson(KeeperPerformanceInfo.NetWorkStat, Formatting.Indented));
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}