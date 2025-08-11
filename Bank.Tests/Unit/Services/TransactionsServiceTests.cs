using Bank.BLL.Services;
using Bank.Core.Entities;
using Bank.Core.Enums;
using Bank.Core.Exceptions;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bank.BLL.Tests.Services
{
    public class TransactionsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<TransactionsService>> _mockLogger;
        private readonly TransactionsService _service;
        private readonly Mock<IAccountsRepository> _mockAccountsRepo;
        private readonly Mock<ITransactionsRepository> _mockTransactionsRepo;

        public TransactionsServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<TransactionsService>>();
            _mockAccountsRepo = new Mock<IAccountsRepository>();
            _mockTransactionsRepo = new Mock<ITransactionsRepository>();
            
            _mockUnitOfWork.Setup(x => x.Accounts).Returns(_mockAccountsRepo.Object);
            _mockUnitOfWork.Setup(x => x.Transactions).Returns(_mockTransactionsRepo.Object);
            
            _service = new TransactionsService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockTransactionsRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTransactions()
        {
            // Arrange
            var expectedTransactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid() },
                new Transaction { Id = Guid.NewGuid() }
            };
            
            _mockTransactionsRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTransactions);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(expectedTransactions, result);
            _mockTransactionsRepo.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TransferFundsAsync_ValidTransfer_CompletesSuccessfully()
        {
            // Arrange
            var senderNumber = "SENDER123";
            var recipientNumber = "RECIPIENT456";
            var amount = 100m;
            var description = "Test transfer";
            
            var senderAccount = new Account { AccountNumber = senderNumber, Balance = 200m };
            var recipientAccount = new Account { AccountNumber = recipientNumber, Balance = 50m };

            _mockAccountsRepo.Setup(x => x.GetByNumberLockedAsync(senderNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(senderAccount);
            _mockAccountsRepo.Setup(x => x.GetByNumberLockedAsync(recipientNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(recipientAccount);

            // Act
            await _service.TransferFundsAsync(senderNumber, recipientNumber, amount, description, CancellationToken.None);

            // Assert
            Assert.Equal(100m, senderAccount.Balance);
            Assert.Equal(150m, recipientAccount.Balance);
            
            _mockTransactionsRepo.Verify(x => x.AddAsync(
                It.Is<Transaction>(t => 
                    t.SenderAccountNumber == senderNumber && 
                    t.RecipientAccountNumber == recipientNumber &&
                    t.Amount == amount),
                It.IsAny<CancellationToken>()),
            Times.Once);
                
            // Logger check
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transfer completed")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task TransferFundsAsync_InsufficientFunds_ThrowsException()
        {
            // Arrange
            var senderNumber = "SENDER123";
            var recipientNumber = "RECIPIENT456";
            var amount = 300m;
            
            var senderAccount = new Account { AccountNumber = senderNumber, Balance = 200m };
            var recipientAccount = new Account { AccountNumber = recipientNumber, Balance = 50m };

            _mockAccountsRepo.Setup(x => x.GetByNumberLockedAsync(senderNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(senderAccount);
            _mockAccountsRepo.Setup(x => x.GetByNumberLockedAsync(recipientNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(recipientAccount);

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => 
                _service.TransferFundsAsync(senderNumber, recipientNumber, amount, null, CancellationToken.None));
                
            _mockUnitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            // Logger check
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transfer failed")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Theory]
        [InlineData("", "RECIPIENT456")]
        [InlineData("SENDER123", "")]
        [InlineData(null, "RECIPIENT456")]
        [InlineData("SENDER123", null)]
        public async Task TransferFundsAsync_EmptyAccountNumber_ThrowsException(string sender, string recipient)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.TransferFundsAsync(sender, recipient, 100m, null, CancellationToken.None));
        }

        [Fact]
        public async Task TransferFundsAsync_SameAccount_ThrowsException()
        {
            // Arrange
            var accountNumber = "ACCOUNT123";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.TransferFundsAsync(accountNumber, accountNumber, 100m, null, CancellationToken.None));
        }

        [Fact]
        public async Task WithdrawFundsAsync_ValidWithdrawal_CompletesSuccessfully()
        {
            // Arrange
            var accountNumber = "ACCOUNT123";
            var amount = 50m;
            
            var account = new Account { AccountNumber = accountNumber, Balance = 100m };

            _mockUnitOfWork.Setup(x => x.Accounts.GetByNumberLockedAsync(accountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            await _service.WithdrawFundsAsync(accountNumber, amount, CancellationToken.None);

            // Assert
            Assert.Equal(50m, account.Balance); // 100 - 50
            
            _mockUnitOfWork.Verify(x => x.Transactions.AddAsync(It.Is<Transaction>(t => 
                t.SenderAccountNumber == accountNumber && 
                t.TypeId == (int)TransactionTypes.Withdrawal &&
                t.Amount == amount), It.IsAny<CancellationToken>()),
            Times.Once);
                
            _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DepositFundsAsync_ValidDeposit_CompletesSuccessfully()
        {
            // Arrange
            var accountNumber = "ACCOUNT123";
            var amount = 75m;

            var account = new Account { AccountNumber = accountNumber, Balance = 100m };

            _mockUnitOfWork.Setup(x => x.Accounts.GetByNumberLockedAsync(accountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            await _service.DepositFundsAsync(accountNumber, amount, CancellationToken.None);

            // Assert
            Assert.Equal(175m, account.Balance); // 100 + 75

            _mockUnitOfWork.Verify(x => x.Transactions.AddAsync(
                It.Is<Transaction>(t =>
                    t.RecipientAccountNumber == accountNumber &&
                    t.TypeId == (int)TransactionTypes.Deposit &&
                    t.Amount == amount),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task TransferFundsAsync_InvalidAmount_ThrowsException(decimal amount)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.TransferFundsAsync("SENDER123", "RECIPIENT456", amount, null, CancellationToken.None));
        }

        [Fact]
        public async Task TransferFundsAsync_AccountNotFound_ThrowsException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Accounts.GetByNumberLockedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.TransferFundsAsync("SENDER123", "RECIPIENT456", 100m, null, CancellationToken.None));
        }

        [Fact]
        public async Task CreateTransaction_ReturnsCorrectTransaction()
        {
            // Arrange
            var type = TransactionTypes.Transfer;
            var sender = "SENDER123";
            var recipient = "RECIPIENT456";
            var amount = 100m;
            var description = "Test";

            // Act
            var transaction = _service.GetType()
                .GetMethod("CreateTransaction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { type, sender, recipient, amount, description }) as Transaction;

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(sender, transaction.SenderAccountNumber);
            Assert.Equal(recipient, transaction.RecipientAccountNumber);
            Assert.Equal(amount, transaction.Amount);
            Assert.Equal(description, transaction.Description);
            Assert.Equal((int)type, transaction.TypeId);
        }
    }
}
