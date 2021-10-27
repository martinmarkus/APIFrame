using APIFrame.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace APIFrame.Web.Request.Filters
{
    public class Antiforgery : ActionFilterAttribute
    {
        private readonly ILogger<Antiforgery> _logger;

        public Antiforgery(ILogger<Antiforgery> logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var antiforgeryFromCookie = context.HttpContext.Request.Cookies[AuthConstants.AntiforgeryCookieToken];
            var antiforgeryFromHeader = context.HttpContext.Request.Headers[AuthConstants.AntiforgeryHeaderToken];

            var metadataList = context.ActionDescriptor.EndpointMetadata;

            // If the endpoint contains IgnoreAntiforgery attribute, ignore the validation
            foreach (var metadata in metadataList)
            {
                if (metadata.GetType() == typeof(IgnoreAntiforgery))
                {
                    await next();
                    return;
                }
            }

            try
            {
                var isTokenChallenged = false;
                if (string.IsNullOrEmpty(antiforgeryFromCookie))
                {
                    _logger.LogInformation("Antiforgery token challenged: token is missing from cookies.");
                    isTokenChallenged = true;
                }
                else if (string.IsNullOrEmpty(antiforgeryFromHeader))
                {
                    _logger.LogInformation("Antiforgery token challenged: token is missing from headers.");
                    isTokenChallenged = true;
                }
                else if (antiforgeryFromCookie.Equals(antiforgeryFromHeader, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Antiforgery token challenged: header and cookie tokens are not matching.");
                    isTokenChallenged = true;
                }

                // Return 401 on missing antiforgery token
                if (isTokenChallenged)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                context.Result = new UnauthorizedResult();
            }

            await next();
        }
    }
}
