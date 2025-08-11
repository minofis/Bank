using Bank.Core.Entities;

namespace Bank.Core.Interfaces.Services
{
    public interface IAccountsService
    {
        Task<List<Account>> GetAllAsync(CancellationToken ct);
        Task<Account> GetByNumberAsync(string accountNumber, CancellationToken ct);
        Task<string> CreateAsync(string holderName, decimal initialBalance, CancellationToken ct);
    }
}