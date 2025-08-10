using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<List<Account>> GetAllAsync(CancellationToken ct);
        Task<Account> GetByNumberAsync(string accountNumber, CancellationToken ct);
        Task<Account> GetByNumberLockedAsync(string accountNumber, CancellationToken ct);
        Task CreateAsync(Account account, CancellationToken ct);
    }
}