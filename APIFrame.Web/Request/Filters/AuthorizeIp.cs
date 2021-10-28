using APIFrame.Core.Constants;
using APIFrame.Core.Models;
using APIFrame.Web.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIFrame.Web.Request.Filters
{
    public class AuthorizeIp : ActionFilterAttribute
    {
        private readonly ILogService _logger;

        public AuthorizeIp(ILogService logger)
        {
            _logger = logger;
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var realIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                var clientIpClaim = context.HttpContext.User?.Claims?
                    .FirstOrDefault(claim => claim.Type.Equals(
                        ClaimConstants.ClientIp,
                        StringComparison.OrdinalIgnoreCase));

                if (clientIpClaim != null && !string.IsNullOrEmpty(clientIpClaim.Value)
                    && !clientIpClaim.Value.Equals(realIp, StringComparison.OrdinalIgnoreCase))
                {
                    await _logger.LogAsync(new Log()
                    {
                        LogLevel = Core.Enums.LogLevel.Info,
                        LogType = Core.Enums.LogType.Authentication,
                        Message = "Authentication was challenged. JWT token was issued for a different client IP."
                    });
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception e)
            {
                await _logger.LogAsync(new Log()
                {
                    LogLevel = Core.Enums.LogLevel.Error,
                    LogType = Core.Enums.LogType.Authentication,
                    Message = e.ToString()
                });
                context.Result = new UnauthorizedResult();
            }

            await next();
        }
    }
}
