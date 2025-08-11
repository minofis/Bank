using Bank.BLL.Services;
using Bank.Core.Entities;
using Bank.Core.Exceptions;
using Bank.Core.Interfaces.Repositories;
using Moq;

namespace Bank.Tests.Unit.Services
{
    public class AccountsServiceTests
    {
        private readonly Mock<IAccountsRepository> _mockRepo;
        private readonly AccountsService _service;

        public AccountsServiceTests()
        {
            _mockRepo = new Mock<IAccountsRepository>();
            _service = new AccountsService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidInput_ReturnsAccountNumber()
        {
            // Arrange
            var holderName = "John Doe";
            var initialBalance = 1000m;
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(holderName, initialBalance);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("ACCT", result);
            Assert.Equal(14, result.Length); // "ACCT" + 10 chars
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_EmptyHolderName_ThrowsArgumentException()
        {
            // Arrange
            var holderName = "";
            var initialBalance = 1000m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateAsync(holderName, initialBalance));
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_NegativeInitialBalance_ThrowsArgumentException()
        {
            // Arrange
            var holderName = "John Doe";
            var initialBalance = -100m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateAsync(holderName, initialBalance));
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAccounts()
        {
            // Arrange
            var expectedAccounts = new List<Account>
            {
                new Account { AccountNumber = "ACCT12345678" },
                new Account { AccountNumber = "ACCT87654321" }
            };
            
            _mockRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccounts);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(expectedAccounts, result);
            _mockRepo.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByNumberAsync_ExistingAccount_ReturnsAccount()
        {
            // Arrange
            var accountNumber = "ACCT12345678";
            var expectedAccount = new Account { AccountNumber = accountNumber };
            
            _mockRepo.Setup(x => x.GetByNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccount);

            // Act
            var result = await _service.GetByNumberAsync(accountNumber);

            // Assert
            Assert.Equal(expectedAccount, result);
            _mockRepo.Verify(x => x.GetByNumberAsync(accountNumber, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByNumberAsync_NonExistingAccount_ThrowsNotFoundException()
        {
            // Arrange
            var accountNumber = "ACCT99999999";
            
            _mockRepo.Setup(x => x.GetByNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.GetByNumberAsync(accountNumber));
            _mockRepo.Verify(x => x.GetByNumberAsync(accountNumber, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GenerateAccountNumber_ReturnsValidFormat()
        {
            // Arrange
            var privateService = new PrivateObject(_service);

            // Act
            var result = (string)privateService.Invoke("GenerateAccountNumber");

            // Assert
            Assert.StartsWith("ACCT", result);
            Assert.Equal(14, result.Length);
            Assert.True(result.Substring(4).All(c => char.IsLetterOrDigit(c) && !char.IsLower(c)));
        }
    }

    // Helper class to test private methods
    public class PrivateObject
    {
        private readonly object _obj;
        
        public PrivateObject(object obj)
        {
            _obj = obj;
        }
        
        public object Invoke(string methodName, params object[] args)
        {
            var method = _obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return method.Invoke(_obj, args);
        }
    }
}