using Bank.API.Controllers;
using Bank.API.DTOs;
using Bank.Core.Entities;
using Bank.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bank.Tests.Unit.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountsService> _mockAccountsService;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _mockAccountsService = new Mock<IAccountsService>();
            _controller = new AccountsController(_mockAccountsService.Object);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsListOfAccounts()
        {
            // Arrange
            var testAccounts = new List<Account>
            {
                new Account 
                { 
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC001",
                    HolderName = "John Doe",
                    Balance = 1000,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockAccountsService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(testAccounts);

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<AccountResponseDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<AccountResponseDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("ACC001", returnValue[0].AccountNumber);
        }

        [Fact]
        public async Task GetAccountByNumber_ReturnsAccount_WhenExists()
        {
            // Arrange
            var testAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "ACC123",
                HolderName = "John Doe",
                Balance = 1000,
                CreatedAt = DateTime.UtcNow
            };

            _mockAccountsService.Setup(x => x.GetByNumberAsync("ACC123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(testAccount);

            // Act
            var result = await _controller.GetAccountByNumber("ACC123");

            // Assert
            var actionResult = Assert.IsType<ActionResult<AccountResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var accountDto = Assert.IsType<AccountResponseDto>(okResult.Value);
            Assert.Equal("ACC123", accountDto.AccountNumber);
        }

        [Fact]
        public async Task GetAccountByNumber_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _mockAccountsService.Setup(x => x.GetByNumberAsync("INVALID", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _controller.GetAccountByNumber("INVALID");

            // Assert
            var actionResult = Assert.IsType<ActionResult<AccountResponseDto>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }


        [Fact]
        public async Task CreateAccount_ReturnsSuccess_WithAccountNumber()
        {
            // Arrange
            var requestDto = new AccountRequestDto
            {
                HolderName = "New User",
                InitialBalance = 500
            };

            _mockAccountsService.Setup(x => x.CreateAsync("New User", 500, It.IsAny<CancellationToken>()))
                .ReturnsAsync("ACC123");

            // Act
            var result = await _controller.CreateAccount(requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<AccountResponseDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenInvalidData()
        {
            // Arrange
            var requestDto = new AccountRequestDto
            {
                HolderName = "",
                InitialBalance = -100
            };

            _controller.ModelState.AddModelError("HolderName", "Required");
            _controller.ModelState.AddModelError("InitialBalance", "Must be positive");

            // Act
            var result = await _controller.CreateAccount(requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<AccountResponseDto>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
    }
}