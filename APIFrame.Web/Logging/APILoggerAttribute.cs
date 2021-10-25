using APIFrame.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace APIFrame.Web.Logging
{
    public class APILogger : ActionFilterAttribute, IActionFilter
    {
        private readonly ILogger<object> _logger;

        public APILogger(ILogger<object> logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                if (!(context.Controller is BaseAPIController))
                {
                    await next();
                    return;
                }

                var request = (context.Controller as BaseAPIController).Request;

                using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
                var bodyContent = reader.ReadToEnd();

                Console.WriteLine(bodyContent);
                _logger.LogInformation(bodyContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _logger.LogError(e.ToString());
            }

            await next();
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            try
            {
                if (!(context.Result is OkObjectResult))
                {
                    await next();
                    return;
                }

                var obkectResult = (context.Result as OkObjectResult).Value;
                var jsonResult = JsonConvert.SerializeObject(obkectResult);

                Console.WriteLine(jsonResult);
                _logger.LogInformation(jsonResult);            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _logger.LogError(e.ToString());
            }

            await next();
        }
    }
}
