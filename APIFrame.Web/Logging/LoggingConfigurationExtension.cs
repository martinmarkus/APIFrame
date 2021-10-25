using APIFrame.Core.Configuration;
using APIFrame.Core.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIFrame.Web.Logging
{
    public static class LoggingConfigurationExtension
    {
        public static void AddCustomSerilog(this IConfiguration config, IWebHostEnvironment env)
        {
            var logOptions = config.GetSection(nameof(LogOptions));
            var customLogContainerPath = logOptions.Get<LogOptions>().LogPath;


            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Async(asyncConfig =>
            {
                var envName = EnvironmentConstants.DEV;

                if (env.EnvironmentName.Equals(EnvironmentConstants.PRODUCTION))
                {
                    envName = EnvironmentConstants.PROD;
                }
                else if (env.EnvironmentName.Equals(EnvironmentConstants.TEST))
                {
                    envName = EnvironmentConstants.TEST;
                }

                var filePath = Path.Combine(customLogContainerPath, "Log-{Date}.log");
                asyncConfig.File(
                    $"{customLogContainerPath}\\Log-{envName.ToUpper()}-",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy.MM.dd-HH:mm:ss.fff}][{Level:u3}][{partnersz}|{partnerNev}|{guestToken}|{requestId}]: {Message:lj}{NewLine}{Exception}");
            })
            .CreateLogger();
        }
    }
}
