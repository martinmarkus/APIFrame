using APIFrame.Core.Models;
using APIFrame.DataAccess.Repositores.Interfaces;
using System.Threading.Tasks;

namespace APIFrame.DataAccess.Repositories.Interfaces
{
    public interface IBaseUserRepo<T> : IAsyncRepo<T> where T : BaseUser
    {
        Task<T> GetByEmailAsync(string userNameOrEmail);
    }
}
