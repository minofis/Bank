using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<List<Account>> GetAllAcountsAsync();
        Task<Account> GetAccountByIdAsync(Guid accountId);
        Task CreateAccountAsync(Account account);
    }
}