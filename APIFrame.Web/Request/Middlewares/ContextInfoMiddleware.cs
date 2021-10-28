using APIFrame.Core.Constants;
using APIFrame.Web.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace APIFrame.Web.Request
{
    public class ContextInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public ContextInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IContextInfo contextInfo)
        {
            var authToken = httpContext.Request?.Cookies[AuthConstants.AuthToken];

            var userIdClaim = httpContext.User?.Claims?
                .FirstOrDefault(claim => claim.Type.Equals(
                    ClaimConstants.UserId,
                    StringComparison.OrdinalIgnoreCase));

            var clientIpClaim = httpContext.User?.Claims?
                .FirstOrDefault(claim => claim.Type.Equals(
                    ClaimConstants.ClientIp,
                    StringComparison.OrdinalIgnoreCase));

            contextInfo.AuthToken = authToken;
            contextInfo.ClientIp = httpContext.Connection?.RemoteIpAddress?.ToString();
            contextInfo.UserId = userIdClaim?.Value ?? AuthConstants.Anonymous;
            contextInfo.RequestDate = DateTime.ParseExact(
                DateTime.Now.ToString(),
                DateTimeConstants.DateFormat,
                CultureInfo.InvariantCulture);

            await _next(httpContext);
        }
    }
}
