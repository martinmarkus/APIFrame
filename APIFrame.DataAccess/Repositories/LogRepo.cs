using APIFrame.Core.Attributes;
using APIFrame.Core.Models;
using APIFrame.DataAccess.DbContexts;
using APIFrame.DataAccess.Repositores;
using APIFrame.DataAccess.Repositories.Interfaces;

namespace APIFrame.DataAccess.Repositories
{
    public class LogRepo : AsyncRepo<Log>, ILogRepo
    {
        public LogRepo(BaseDbContext dbContext) : base(dbContext)
        {
        }
    }
}
