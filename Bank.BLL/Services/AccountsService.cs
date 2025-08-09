using Bank.Core.Entities;
using Bank.Core.Interfaces.Repositories;
using Bank.Core.Interfaces.Services;

namespace Bank.BLL.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _accountsRepo;
        private readonly ITransactionsService _transactionsService;

        public AccountsService(IAccountsRepository accountsRepo, ITransactionsService transactionsService)
        {
            _accountsRepo = accountsRepo;
            _transactionsService = transactionsService;
        }

        public async Task CreateAsync(string holderName, decimal initialBalance)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Number = GenerateAccountNumber(),
                HolderName = holderName,
                Balance = 0
            };

            await _accountsRepo.CreateAsync(account);

            await _transactionsService.DepositFundsAsync(account.Number, initialBalance);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _accountsRepo.GetAllAsync();
        }

        public async Task<Account> GetByNumberAsync(string accountNumber)
        {
            return await _accountsRepo.GetByNumberAsync(accountNumber)
                ?? throw new ArgumentException($"Account with number {accountNumber} not found.");
        }

        private string GenerateAccountNumber()
        {
            return "ACCT" + DateTime.Now.Ticks.ToString().Substring(0, 10);
        }
    }
}