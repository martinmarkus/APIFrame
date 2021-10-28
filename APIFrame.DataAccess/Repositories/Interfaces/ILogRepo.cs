using APIFrame.Core.Attributes;
using APIFrame.Core.Models;
using APIFrame.DataAccess.Repositores.Interfaces;

namespace APIFrame.DataAccess.Repositories.Interfaces
{
    [ExceptDynamicResolve]
    public interface ILogRepo : IAsyncRepo<Log>
    {
    }
}
