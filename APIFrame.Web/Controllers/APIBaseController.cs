using APIFrame.Web.Logging;
using Microsoft.AspNetCore.Mvc;

namespace APIFrame.Web.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [ExceptionHandling.ExceptionHandler]
    [ServiceFilter(typeof(APILogger))]
    public abstract class APIBaseController : ControllerBase
    {
    }
}
