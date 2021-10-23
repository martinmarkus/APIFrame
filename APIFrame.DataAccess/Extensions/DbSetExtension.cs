using Microsoft.EntityFrameworkCore;

namespace APIFrame.DataAccess.Extensions
{
    public static class DbSetExtension
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
