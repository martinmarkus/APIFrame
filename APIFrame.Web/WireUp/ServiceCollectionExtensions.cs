using AspNetCoreRateLimit;
using APIFrame.BackgroundService.Services;
using APIFrame.BackgroundService.Services.Interfaces;
using APIFrame.Core.Utils;
using APIFrame.Web.Logging;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;

namespace APIFrame.Web.WireUp
{
    public static class ServiceCollontionExtension
    {
        public static void AddCustomBaseTypes(this IServiceCollection services)
        {
            //services.ResolveDynamically("APIFrame.Web.Services.Interfaces", "APIFrame.Web.Services");

            services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            services.AddScoped<StringGeneratorUtil>();
            services.AddScoped<APILogger>();
        }

        public static void AddCustomAuthentication(
           this IServiceCollection services,
           byte[] secretKey,
           string issuer)
        {
            services.AddAuthentication(oOptions =>
            {
                oOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                oOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(oOptions =>
                {
                    oOptions.SaveToken = true;
                    oOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void AddCustomSwagger(this IServiceCollection services, string apiName)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = apiName,
                    Version = "v1",
                    Description = string.Empty
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public static void AddCustomHangfire(this IServiceCollection services)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());

            services.AddHangfireServer();
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddOptions();

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }

        public static T ConfigureOption<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, new()
        {
            var options = new T();

            configuration.GetSection(typeof(T).Name).Bind(options);
            services.Configure<T>(configuration.GetSection(typeof(T).Name));
            services.AddSingleton(options);

            return options;
        }
    }
}
