using Bank.Core.Entities;
using Bank.Core.Enums;
using Bank.Core.Lookups;
using Bank.DAL;
using Bank.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bank.Tests.Unit.Repositories
{
    public class TransactionsRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly TransactionsRepository _repository;

        public TransactionsRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new TransactionsRepository(_context);
            
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTransactions()
        {
            // Arrange
            var expectedTransactions = new List<Transaction>
            {
                new Transaction { 
                    Id = Guid.NewGuid(), 
                    Amount = 100,
                    Description = "Test 1",
                    TypeId = (int)TransactionTypes.Deposit
                },
                new Transaction { 
                    Id = Guid.NewGuid(), 
                    Amount = 200,
                    Description = "Test 2",
                    TypeId = (int)TransactionTypes.Withdrawal
                }
            };

            await _context.Transactions.AddRangeAsync(expectedTransactions);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedTransactions[0].Id, result[0].Id);
            Assert.Equal(expectedTransactions[1].Id, result[1].Id);
        }

        [Fact]
        public async Task AddAsync_AddsTransactionToDatabase()
        {
            // Arrange
            var transaction = new Transaction 
            { 
                Id = Guid.NewGuid(),
                Amount = 100,
                Description = "Test transaction",
                TypeId = (int)TransactionTypes.Transfer,
                SenderAccountNumber = "ACC123",
                RecipientAccountNumber = "ACC456"
            };

            // Act
            await _repository.AddAsync(transaction);
            await _context.SaveChangesAsync();

            // Assert
            var dbTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);
                
            Assert.NotNull(dbTransaction);
            Assert.Equal(transaction.Amount, dbTransaction.Amount);
            Assert.Equal(transaction.Description, dbTransaction.Description);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}