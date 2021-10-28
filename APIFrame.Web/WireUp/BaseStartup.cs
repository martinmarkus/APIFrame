using AspNetCoreRateLimit;
using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.WireUp;
using APIFrame.Core.Configuration;
using APIFrame.Core.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using APIFrame.Web.Logging;
using Microsoft.AspNetCore.Http;
using APIFrame.DataAccess.Repositories.Interfaces;
using APIFrame.DataAccess.Repositories;

namespace APIFrame.Web.WireUp
{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration { get; }

        protected IWebHostEnvironment Environment { get; private set; }

        protected string AppName { get; set; }

        protected BaseOptions BaseOptions { get; private set; }

        public BaseStartup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;

            var logOptions = Configuration.GetSection(nameof(LogOptions));
            var customLogContainerPath = logOptions.Get<LogOptions>().LogPath;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(
                    $"appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{EnvironmentConstants.DEVELOPMENT}.json",
                    optional: false,
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{EnvironmentConstants.TEST}.json",
                    optional: false,
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{EnvironmentConstants.PRODUCTION}.json",
                    optional: false,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var baseSection = Configuration.GetSection(OptionConstants.BASE_OPTIONS);
            BaseOptions = baseSection.Get<BaseOptions>();

            AppName = BaseOptions.AppName;

            // INFO: Alapvető konfigurációk
            services.AddConfiguration<BaseOptions>(Configuration);
            services.AddConfiguration<LogOptions>(Configuration);
            services.AddConfiguration<JobConfigurationOptions>(Configuration);

            // INFO: Alapvető webes függőségek
            services.AddDefaultServices();

            // INFO: Rate limiting
            var rateLimitingConfig = Configuration.GetSection(OptionConstants.RATELIMITS);
            services.AddRateLimiting(Configuration);
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            // INFO: Add Serilog logging
            Configuration.AddCustomSerilog(Environment);

            // INFO: Allow sync calls for IIS and Kestrel
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // INFO: Alap Cors policy
            if (BaseOptions.UseDefaultCorsPolicies)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(OptionConstants.CUSTOM_POLICY, builder =>
                    {
                        builder.AllowAnyHeader()
                               .AllowAnyMethod()
                               .WithOrigins(BaseOptions.AllowedCORSPolicyURLs)
                               .AllowCredentials();
                    });
                });
            }

            // INFO: Alap autentikáció
            if (BaseOptions.UseDefaultAuthentication)
            {
                services.AddCustomAuthentication(
                    Encoding.ASCII.GetBytes(BaseOptions.AuthSecretKey),
                    BaseOptions.AuthIssuerName);
            }

            // INFO: Swagger
            if (BaseOptions.UseDefaultSwagger && IsDevelopmentEnvironment())
            {
                services.AddCustomSwagger(AppName);
            }

            // INFO: BackgroundJob függőségek
            if (BaseOptions.UseBackgroundJobs)
            {
                var jobConfigSection = Configuration.GetSection(OptionConstants.JOB_CONFIGURATION_OPTIONS);
                var jobConfigOptions = jobConfigSection.Get<JobConfigurationOptions>();
                services.AddJobConfigurations(jobConfigOptions.JobConfigPath);
                services.AddBackgroundJobSerice();
                services.AddCustomHangfire();
            }
        }

        public virtual void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            var isDevEnv = IsDevelopmentEnvironment();

            // INFO: For nginx hosting
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCustomExceptionHandling();
            app.UseBaseServices();

            if (BaseOptions.UseDefaultCorsPolicies)
            {
                app.UseCors(OptionConstants.CUSTOM_POLICY);
            }

            if (BaseOptions.UseDefaultAuthentication)
            {
                app.UseAuthentication();
            }

            if (BaseOptions.UseDefaultSwagger && isDevEnv)
            {
                app.UseCustomSwagger(AppName);
            }

            if (BaseOptions.UseBackgroundJobs)
            {
                app.UseCustomHangfire();
            }

            if (!isDevEnv)
            {
                app.UseHsts();
            }

            if (isDevEnv)
            {
                app.UseDeveloperExceptionPage();
            }
        }

        protected virtual bool IsDevelopmentEnvironment() =>
            Environment.EnvironmentName.Equals(
                EnvironmentConstants.DEVELOPMENT,
                StringComparison.OrdinalIgnoreCase);
    }
}
