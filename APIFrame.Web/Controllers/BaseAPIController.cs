using APIFrame.Web.Logging;
using Microsoft.AspNetCore.Mvc;

namespace APIFrame.Web.Controllers
{
    /// <summary>
    /// APIController, which includes basic exception handling and logging features.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [ExceptionHandling.ExceptionHandler]
    [ServiceFilter(typeof(APILogger))]
    public abstract class BaseAPIController : ControllerBase
    {
    }
}
