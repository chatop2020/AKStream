using System;
using System.Security.Cryptography;
using System.Text;
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
        public static string MD5Encrypt32(string source)

        {
            string rule = "";

            MD5 md5 = MD5.Create();

            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(source));

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得

            for (int i = 0; i < s.Length; i++)

            {
                rule = rule + s[i].ToString("x2"); // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
            }


            return rule;
        }


        public static PerformanceInfo KeeperPerformanceInfo = new PerformanceInfo();
        private static SystemInfo _keeperSystemInfo = new SystemInfo();

        static void Main(string[] args)
        {
            Console.WriteLine(MD5Encrypt32("defaultuser:default:defaultpasswd"));
            return;

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