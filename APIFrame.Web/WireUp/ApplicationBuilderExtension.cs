using APIFrame.Web.Request;
using APIFrame.Web.Request.Middlewares;
using AspNetCoreRateLimit;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Threading.Tasks;

namespace APIFrame.Web.WireUp
{
    public static class ApplicationBuilderExtension
    {
        public static void UseCustomSwagger(this IApplicationBuilder app, string appName)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appName} V1");
                c.DocExpansion(DocExpansion.None);
                c.RoutePrefix = string.Empty;
            });
        }

        public static void UseCustomHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }

        public static void UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                await Task.Run(() =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = feature.Error;

                    var result = JsonConvert.SerializeObject(new { error = exception.Message });
                    context.Response.ContentType = "application/json";
                });
            }));
        }

        public static void UseBaseServices(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ProfilingMiddleware>();
            app.UseMiddleware<ContextInfoMiddleware>();

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
