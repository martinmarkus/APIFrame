using APIFrame.Core.Constants;
using APIFrame.Web.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
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

        public async Task InvokeAsync(HttpContext httpContext, IAuthContextInfo authContextInfo)
        {
            var authToken = httpContext.Request?.Cookies[AuthConstants.AuthToken];
            var antiforgeryToken = httpContext.Request?.Cookies[AuthConstants.AntiforgeryToken];

            var userIdClaim = httpContext.User?.Claims?
                .First(claim => claim.Type.Equals(
                    ClaimConstants.UserId,
                    StringComparison.OrdinalIgnoreCase));

            authContextInfo.AuthToken = authToken;
            authContextInfo.AntiforgeryToken = antiforgeryToken;
            authContextInfo.ClientIp = httpContext.Connection?.RemoteIpAddress?.ToString();
            authContextInfo.UserId = userIdClaim?.Value;

            await _next(httpContext);
        }
    }
}
