using System;

namespace APIFrame.DataAccess.Transactions
{
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}
