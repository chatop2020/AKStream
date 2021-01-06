using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AKStreamWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Common.Init();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls($"http://*:{Common.AkStreamWebConfig.WebApiPort}");
                });
    }
}