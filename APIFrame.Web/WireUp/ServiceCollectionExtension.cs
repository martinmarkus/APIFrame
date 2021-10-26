using AspNetCoreRateLimit;
using APIFrame.Core.Utils;
using APIFrame.Web.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using APIFrame.Web.Conventions;
using System.Collections.Generic;
using System.Linq;
using APIFrame.Core.Configuration;
using APIFrame.Web.Authentication;
using APIFrame.Web.Authentication.Interfaces;

namespace APIFrame.Web.WireUp
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers essential web dependencies, utils and logging service.
        /// </summary>
        /// <param name="services"></param>
        public static void AddDefaultServices(this IServiceCollection services)
        {
            services.ResolveDynamically(Assembly.GetAssembly(typeof(ServiceCollectionExtension)));
            services.AddScoped<StringGeneratorUtil>();
            services.AddScoped<APILogger>();
            services.AddScoped<IAuthContextInfo, AuthContextInfo>();

            services.AddControllers();
            services.AddHttpClient();

            services.AddMvc(options =>
            {
                options.Conventions.Add(new ControllerNameAttributeConvention());
                options.EnableEndpointRouting = false;
            });
        }

        /// <summary>
        /// Registers the basic JWT authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secretKey"></param>
        /// <param name="issuer"></param>
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

        /// <summary>
        /// Registers swagger service for development and test environments.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiName"></param>
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

        /// <summary>
        /// Registers AspNetCoreRateLimiting for rate limiting solution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddRateLimiting(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddOptions();

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        /// <summary>
        /// Registers the given T type as configuration for scoped and singleton usage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, new()
        {
            var options = new T();

            configuration.GetSection(typeof(T).Name).Bind(options);
            services.Configure<T>(configuration.GetSection(typeof(T).Name));
            services.AddSingleton(options);
        }

        /// <summary>
        /// Registers every classes as configuration type for scoped and singleton usage.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="namespace"></param>
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration, string @namespace)
        {
            var resultTypes = new List<Type>();

            var entryTypes = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .ToList();

            resultTypes.AddRange(entryTypes);

            var referencedAssemblies = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies();

            foreach (var assemblyName in referencedAssemblies)
            {
                var types = Assembly
                    .Load(assemblyName)
                    .GetTypes();

                resultTypes.AddRange(types);
            }

            foreach (var type in resultTypes)
            {
                if (type.IsClass && !string.IsNullOrEmpty(type.Namespace) 
                    && type.BaseType.Equals(typeof(BaseOptions))
                    && type.Namespace.Equals(@namespace, StringComparison.OrdinalIgnoreCase))
                {
                    // INFO: Megtalált Config típus regisztrálása
                    var configSection = configuration.GetSection(type.Name);
                    var config = configSection.Get(type);
                    services.ConfigureOptions(config);
                    services.AddSingleton(config);
                }
            }
        }
    }
}
