using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface ITransactionsRepository
    {
        Task CreateTransactionAsync(Transaction transaction);
    }
}