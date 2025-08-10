using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface ITransactionsRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken ct);
    }
}