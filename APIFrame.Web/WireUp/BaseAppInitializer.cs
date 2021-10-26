using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace APIFrame.Web.WireUp
{
    public class BaseAppInitializer
    {
        public static void StartWithDefaultBuilderAndLogging<T>(string[] baseArgs)
            where T : BaseStartup
        {
            try
            {
                Host.CreateDefaultBuilder(baseArgs)
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("rate_limits.json", optional: false);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<T>();
                    })
                    .UseSerilog()
                    .ConfigureLogging((context, builder) =>
                    {
                        builder.ClearProviders();
                    })
                    .Build()
                    .Run();
            }
            catch
            {
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
