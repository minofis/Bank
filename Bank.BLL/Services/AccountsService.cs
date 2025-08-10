using Bank.Core.Entities;
using Bank.Core.Interfaces.Repositories;
using Bank.Core.Interfaces.Services;

namespace Bank.BLL.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _accountsRepo;

        public AccountsService(IAccountsRepository accountsRepo)
        {
            _accountsRepo = accountsRepo;
        }

        public async Task CreateAsync(string holderName, decimal initialBalance, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(holderName))
                throw new ArgumentException("Holder name cannot be empty.");

            if (initialBalance < 0)
                throw new ArgumentException("Initial balance must be positive.");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                AccountNumber = GenerateAccountNumber(),
                HolderName = holderName,
                Balance = initialBalance
            };

            await _accountsRepo.CreateAsync(account, ct);
        }

        public async Task<List<Account>> GetAllAsync(CancellationToken ct = default)
        {
            return await _accountsRepo.GetAllAsync(ct);
        }

        public async Task<Account> GetByNumberAsync(string accountNumber, CancellationToken ct = default)
        {
            return await _accountsRepo.GetByNumberAsync(accountNumber, ct)
                ?? throw new ArgumentException($"Account with number {accountNumber} not found.");
        }

        private string GenerateAccountNumber()
        {
            return "ACCT" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        }
    }
}