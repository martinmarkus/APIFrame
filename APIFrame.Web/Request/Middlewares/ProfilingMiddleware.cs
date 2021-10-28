using APIFrame.Core.Constants;
using APIFrame.Web.Authentication.Interfaces;
using APIFrame.Web.Logging.Interfaces;
using APIFrame.Web.Request.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace APIFrame.Web.Request.Middlewares
{
    public class ProfilingMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfilingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            IContextInfo contextInfo,
            IRequestProfiler requestProfiler,
            ILogService logService)
        {
            requestProfiler.Start();

            await _next(httpContext);

            contextInfo.ElapsedMs = requestProfiler.Stop();

            await logService.LogAsync(new Core.Models.Log()
            {
                LogType = Core.Enums.LogType.Web,
                Executor = contextInfo.UserId ?? AuthConstants.Anonymous,
                Message = $"'{httpContext.Request.Method.ToUpper()}' request has finished " +
                    $"({httpContext.Response.StatusCode}) after {contextInfo.ElapsedMs} ms."
            });
        }
    }
}
