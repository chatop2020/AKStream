using LibCommon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AKStreamWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tmpRet = UtilsHelper.GetMainParams(args);
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
                    GCommon.OutLogPath += "/";
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
                    if (string.IsNullOrEmpty(Common.AkStreamWebConfig.ListenIp))
                    {
                        webBuilder.UseStartup<Startup>().UseUrls($"http://*:{Common.AkStreamWebConfig.WebApiPort}");
                    }
                    else
                    {
                        var url = $"http://{Common.AkStreamWebConfig.ListenIp}:{Common.AkStreamWebConfig.WebApiPort}";
                        webBuilder.UseStartup<Startup>().UseUrls(url);
                    }
                });
    }
}