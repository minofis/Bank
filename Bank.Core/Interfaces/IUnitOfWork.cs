using System.Data;
using Bank.Core.Interfaces.Repositories;

namespace Bank.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountsRepository Accounts { get; }
        ITransactionsRepository Transactions { get; }
        Task CommitAsync(CancellationToken ct);
        Task RollbackAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken ct);
    }
}