using APIFrame.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace APIFrame.DataAccess.Transactions
{
    public class DatabaseTransaction<T> : IDatabaseTransaction where T: BaseDbContext
    {
        private IDbContextTransaction _transaction;

        public DatabaseTransaction(T context)
        {
            _transaction = context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
