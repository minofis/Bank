
using Bank.Core.Entities;
using Bank.DAL;
using Bank.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bank.Tests.Unit.Repositories
{
    public class AccountsRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly AccountsRepository _repository;

        public AccountsRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new AccountsRepository(_context);
            
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private Account CreateTestAccount(string accountNumber = "ACC123", decimal balance = 1000)
        {
            return new Account 
            { 
                Id = Guid.NewGuid(),
                AccountNumber = accountNumber,
                Balance = balance,
                HolderName = "Test Holder",
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAccounts()
        {
            // Arrange
            var account1 = CreateTestAccount("ACC001");
            var account2 = CreateTestAccount("ACC002");
            
            await _context.Accounts.AddRangeAsync(account1, account2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.AccountNumber == "ACC001");
            Assert.Contains(result, a => a.AccountNumber == "ACC002");
        }

        [Fact]
        public async Task CreateAsync_AddsAccountToDatabase()
        {
            // Arrange
            var account = CreateTestAccount();

            // Act
            await _repository.CreateAsync(account);

            // Assert
            var dbAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
            Assert.NotNull(dbAccount);
            Assert.Equal(account.AccountNumber, dbAccount.AccountNumber);
            Assert.Equal(account.Balance, dbAccount.Balance);
            Assert.Equal(account.HolderName, dbAccount.HolderName);
        }

        [Fact]
        public async Task GetByNumberAsync_ReturnsCorrectAccount()
        {
            // Arrange
            var account = CreateTestAccount();
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNumberAsync("ACC123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(account.Id, result.Id);
            Assert.Equal(account.AccountNumber, result.AccountNumber);
        }

        [Fact]
        public async Task GetByNumberAsync_ReturnsNullForNonExistingAccount()
        {
            // Act
            var result = await _repository.GetByNumberAsync("NON_EXISTING");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNumberLockedAsync_ReturnsNullForNonExistingAccount()
        {
            // Act
            var result = await _repository.GetByNumberLockedAsync("NON_EXISTING");

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}