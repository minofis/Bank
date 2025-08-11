using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface ITransactionsRepository
    {
        Task<List<Transaction>> GetAllAsync(CancellationToken ct);
        Task AddAsync(Transaction transaction, CancellationToken ct);
    }
}