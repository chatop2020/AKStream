using System;
using LibCommon;
using LibLogger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AKStreamKeeper
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if (DEBUG)
            Common.IsDebug = true;
#endif

          var tmpRet=  UtilsHelper.GetMainParams(args);
          if (tmpRet != null && tmpRet.Count > 0)
          {
              foreach (var tmp in tmpRet)
              {
                  if (tmp.Key.ToUpper().Equals("-C"))
                  {
                      GCommon.OutConfigPath = tmp.Value;
                  }
                  if (tmp.Key.ToUpper().Equals("-L"))
                  {
                      GCommon.OutLogPath = tmp.Value;
                  }
              }
          }

          if (!string.IsNullOrEmpty(GCommon.OutLogPath))
          {
              if (!GCommon.OutLogPath.Trim().EndsWith('/'))
              {
                  GCommon.OutLogPath +=  "/";
              }
            
          }
          GCommon.InitLogger();
            Common.Init();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls($"http://*:{Common.AkStreamKeeperConfig.WebApiPort}");
                });
    }
}