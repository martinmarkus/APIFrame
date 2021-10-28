using APIFrame.Core.Attributes;
using APIFrame.Core.Models;
using System.Threading.Tasks;

namespace APIFrame.Web.Logging.Interfaces
{
    [ExceptDynamicResolve]
    public interface ILogService
    {
        Task LogAsync(Log log);
    }
}
