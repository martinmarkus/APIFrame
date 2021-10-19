using APIFrame.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APIFrame.Web.WireUp
{
    public class BaseAppInitializer
    {
        public static IHostBuilder CreateHostBuilder<T>(string[] args) where T : BaseStartup =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<T>();
                });

        public static IHostBuilder CreateHostBuilderWithLogging<T>(string[] baseArgs) where T : BaseStartup =>
            Host.CreateDefaultBuilder(baseArgs)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();

                    var baseOptions = context.Configuration.GetSection("BaseOptions");
                    var customLogContainerPath = baseOptions.Get<BaseOptions>().CustomLogContainerPath;

                    logging.AddFile(customLogContainerPath);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<T>();
                });
    }
}
