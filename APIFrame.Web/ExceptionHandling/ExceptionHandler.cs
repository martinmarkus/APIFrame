using APIFrame.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace APIFrame.Web.ExceptionHandling
{
    public class ExceptionHandler : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext context)
        {
            var code = context.Exception switch
            {
                NotFoundException _ => HttpStatusCode.NotFound,
                UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                BannedUserException _ => HttpStatusCode.Forbidden,
                NullReferenceException _ => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.BadRequest,
            };

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)code;

            context.Result = new JsonResult(new
            {
                status = code,
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });
        }
    }
}
