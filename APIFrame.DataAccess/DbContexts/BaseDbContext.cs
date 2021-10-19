using APIFrame.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFrame.DataAccess.DbContexts
{
    public class BaseDbContext : DbContext
    {
        public virtual DbSet<BaseUser> BaseUsers { get; set; }

        public BaseDbContext(DbContextOptions<BaseDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
