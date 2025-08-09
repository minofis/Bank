using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account> GetByNumberAsync(string accountNumber);
        Task<Account> GetByNumberLockedAsync(string accountNumber);
        Task CreateAsync(Account account);
    }
}