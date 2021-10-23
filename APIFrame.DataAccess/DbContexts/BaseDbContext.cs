using APIFrame.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFrame.DataAccess.DbContexts
{
    public abstract class BaseDbContext : DbContext
    {
        public abstract DbSet<BaseUser> BaseUsers { get; set; }

        public BaseDbContext(DbContextOptions<BaseDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
