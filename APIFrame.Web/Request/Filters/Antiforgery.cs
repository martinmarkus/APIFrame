using APIFrame.Core.Constants;
using APIFrame.Core.Models;
using APIFrame.Web.Authentication.Interfaces;
using APIFrame.Web.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace APIFrame.Web.Request.Filters
{
    public class Antiforgery : ActionFilterAttribute
    {
        private readonly IAntiforgeryService _antiforgeryToken;
        private readonly ILogService _logger;

        public Antiforgery(
             IAntiforgeryService antiforgeryToken,
             ILogService logger)
        {
            _antiforgeryToken = antiforgeryToken ?? throw new ArgumentException(nameof(antiforgeryToken));
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
                var validationResult =_antiforgeryToken.ValidateAntiforgeryTokens(antiforgeryFromCookie, antiforgeryFromHeader);

                // Returns 401 on missing antiforgery token
                if (validationResult != Core.Enums.AntiforgeryValidation.Valid)
                {
                    await _logger.LogAsync(new Log()
                    {
                        LogLevel = Core.Enums.LogLevel.Critical,
                        LogType = Core.Enums.LogType.Antiforgery,
                        Message = $"Antiforgery token validation was challenged: {validationResult}"
                    });
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception e)
            {
                await _logger.LogAsync(new Log()
                {
                    LogLevel = Core.Enums.LogLevel.Critical,
                    LogType = Core.Enums.LogType.Antiforgery,
                    Message = e.ToString()
                });
                context.Result = new UnauthorizedResult();
            }

            await next();
        }
    }
}
