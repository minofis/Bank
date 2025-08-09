using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Services
{
    public interface IAccountsService
    {
        Task<List<Account>> GetAllAsync();
        Task<Account> GetByNumberAsync(string accountNumber);
        Task CreateAsync(string holderName, decimal initialBalance);
    }
}