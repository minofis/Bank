using System.Data;
using Bank.Core.Interfaces.Repositories;

namespace Bank.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountsRepository Accounts { get; }
        ITransactionsRepository Transactions { get; }
        Task CommitAsync();
        Task RollbackAsync();
        Task BeginTransactionAsync();
    }
}