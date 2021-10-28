using APIFrame.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFrame.DataAccess.DbContexts
{
    public abstract class BaseDbContext : DbContext
    {
        public virtual DbSet<BaseUser> Users { get; set; } 

        public virtual DbSet<Log> Logs { get; set; }

        public BaseDbContext(DbContextOptions<BaseDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
