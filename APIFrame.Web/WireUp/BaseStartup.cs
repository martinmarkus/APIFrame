using AspNetCoreRateLimit;
using APIFrame.BackgroundService.Configuration;
using APIFrame.BackgroundService.WireUp;
using APIFrame.BackgroundService.WireUp.Interfaces;
using APIFrame.Core.Configuration;
using APIFrame.Core.Constants;
using APIFrame.Web.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace APIFrame.Web.WireUp

{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration { get; }

        protected string _appName;

        public BaseStartup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(
                    $"appsettings.{env.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{env.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var corsSection = Configuration.GetSection(OptionConstants.ALLOWED_CORS_POLICY_URLS);
            var CORSPolicies = corsSection.Get<string[]>();

            var baseSection = Configuration.GetSection(OptionConstants.BASE_OPTIONS);
            var baseOptions = baseSection.Get<BaseOptions>();
            _appName = baseOptions.AppName;

            services.ConfigureOption<BaseOptions>(Configuration);
            services.ConfigureOption<JobConfigurationOptions>(Configuration);

            services.AddControllers();
            services.AddHttpClient();
            services.AddCustomHangfire();
            services.AddCustomSwagger(_appName);

            services.AddScoped<IJobConfigurationResolver, JobConfigurationResolver>();

            services.AddCustomBaseTypes();

            var rateLimitingConfig = Configuration.GetSection(OptionConstants.RATELIMITS);
            services.AddRateLimiting(Configuration);
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddCors(options =>
            {
                options.AddPolicy(OptionConstants.CUSTOM_POLICY, builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithOrigins(CORSPolicies)
                           .AllowCredentials();
                });
            });

            services.AddMvc(options =>
            {
                options.Conventions.Add(new ControllerNameAttributeConvention());
                options.EnableEndpointRouting = false;
            });

            // INFO: Allow sync calls for IIS and Kestrel
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IJobConfigurationResolver resolver)
        {
            // INFO: For nginx hosting
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.EnvironmentName.Equals(EnvironmentConstants.DEVELOPMENT, StringComparison.OrdinalIgnoreCase))
            {
                app.UseCustomSwagger(_appName);
            }

            app.UseCustomExceptionHandling();
            app.UseCustomHangfire();
            app.UseBaseServices();
        }
    }
}
