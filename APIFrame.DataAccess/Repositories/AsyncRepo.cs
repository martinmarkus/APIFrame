using APIFrame.Core.Models;
using APIFrame.DataAccess.DbContexts;
using APIFrame.DataAccess.Repositores.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFrame.DataAccess.Repositores
{
    public abstract class AsyncRepo<T> : IAsyncRepo<T> where T : BaseEntity
    {
        protected readonly BaseDbContext _dbContext;

        public AsyncRepo(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            T existingEntity = await _dbContext.Set<T>()
                .FindAsync(entity.Id);

            if (existingEntity == null)
            {
                var addedEntity = await _dbContext.Set<T>()
                    .AddAsync(entity);

                await SaveChangesAsync();

                return addedEntity.Entity;
            }

            return default;
        }

        public virtual async Task AddRangeAsync(IList<T> entities)
        {
            await _dbContext.Set<T>()
                .AddRangeAsync(entities);

            await SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsByIdAsync(Guid id)
        {
            var entity = await _dbContext.Set<T>()
                .FirstOrDefaultAsync(entity => entity.Id == id && entity.IsActive);

            return entity != null;
        }

        public virtual async Task<IList<T>> GetAllAsync() =>
            await _dbContext.Set<T>()
                .Where(entity => entity.IsActive)
                .OrderBy(entity => entity.CreationDate)
                .ToListAsync();

        public virtual async Task<T> GetByIdAsync(Guid id) =>
            await _dbContext.Set<T>()
                .FirstOrDefaultAsync(entity => entity.Id == id && entity.IsActive);

        public virtual async Task RemoveAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                entity.IsActive = false;
                _dbContext.Entry(entity).CurrentValues.SetValues(entity);
                await SaveChangesAsync();
            }
        }

        public virtual async Task RemoveAsync(T entity)
        {
            if (entity != null)
            {
                entity.IsActive = false;
                _dbContext.Entry(entity).CurrentValues.SetValues(entity);
                await SaveChangesAsync();
            }
        }

        public async Task RemoveAllAsync(IList<T> entities)
        {
            if (entities != null)
            {
                _dbContext.RemoveRange(entities);

                foreach (var entity in entities)
                {
                    RemoveWithoutSave(entity);
                }

                await SaveChangesAsync();
            }
        }

        protected virtual void RemoveWithoutSave(T entity)
        {
            if (entity != null)
            {
               entity.IsActive = false;
                _dbContext.Entry(entity).CurrentValues.SetValues(entity); 
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            T existing = await _dbContext
                .Set<T>()
                .FindAsync(entity.Id);

            if (existing != null && entity.IsActive)
            {
                _dbContext.Entry(existing)
                    .CurrentValues
                    .SetValues(entity);

                await SaveChangesAsync();
            }
        }

        public virtual async Task UpdateRangeAsync(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                T existing = await _dbContext
                    .Set<T>()
                    .FindAsync(entity.Id);

                if (existing != null && entity.IsActive)
                {
                    _dbContext.Entry(existing)
                        .CurrentValues
                        .SetValues(entity);
                }
            }

            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
