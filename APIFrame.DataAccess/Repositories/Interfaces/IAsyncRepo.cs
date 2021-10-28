using APIFrame.Core.Attributes;
using APIFrame.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIFrame.DataAccess.Repositores.Interfaces
{
    [ExceptDynamicResolve]
    public interface IAsyncRepo<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id);

        Task<IList<T>> GetAllAsync();

        Task<T> AddAsync(T entity);

        Task AddRangeAsync(IList<T> entity);

        Task UpdateAsync(T entity);

        Task UpdateRangeAsync(IList<T> entities);

        Task RemoveAsync(Guid id);

        Task RemoveAsync(T entity);

        Task RemoveAllAsync(IList<T> entities);

        Task<bool> ExistsByIdAsync(Guid id);

        Task SaveChangesAsync();
    }
}
