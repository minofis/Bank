using Bank.API.Controllers;
using Bank.API.DTOs.TransactionDTOs;
using Bank.Core.Entities;
using Bank.Core.Interfaces.Services;
using Bank.Core.Lookups;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bank.Tests.Unit.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionsService> _mockTransactionsService;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _mockTransactionsService = new Mock<ITransactionsService>();
            _controller = new TransactionsController(_mockTransactionsService.Object);
        }

        [Fact]
        public async Task GetAllTransactions_ReturnsListOfTransactions()
        {
            // Arrange
            var testTransactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 100,
                    Description = "Test 1",
                    SenderAccountNumber = "ACC001",
                    RecipientAccountNumber = "ACC002",
                    Timestamp = DateTime.UtcNow,
                    Type = new TransactionType { Name = "Transfer" }
                }
            };

            _mockTransactionsService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(testTransactions);

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<TransactionResponseDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<TransactionResponseDto>>(okResult.Value);
            
            Assert.Single(returnValue);
            Assert.Equal("ACC001", returnValue[0].SenderAccountNumber);
            Assert.Equal("Transfer", returnValue[0].Type);
        }

        [Fact]
        public async Task TransferFunds_ReturnsSuccessResponse()
        {
            // Arrange
            var requestDto = new TransferFundsRequestDto
            {
                SenderAccountNumber = "ACC001",
                RecipientAccountNumber = "ACC002",
                Amount = 100,
                Description = "Test transfer"
            };

            // Act
            var result = await _controller.TransferFunds(requestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockTransactionsService.Verify(x => x.TransferFundsAsync(
                "ACC001", "ACC002", 100, "Test transfer", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawFunds_ReturnsSuccessResponse()
        {
            // Arrange
            var requestDto = new WithdrawFundsRequestDto
            {
                AccountNumber = "ACC001",
                Amount = 100
            };

            // Act
            var result = await _controller.WithdrawFunds(requestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockTransactionsService.Verify(x => x.WithdrawFundsAsync(
                "ACC001", 100, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DepositFunds_ReturnsSuccessResponse()
        {
            // Arrange
            var requestDto = new DepositFundsRequestDto
            {
                AccountNumber = "ACC001",
                Amount = 100
            };

            // Act
            var result = await _controller.DepositFunds(requestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockTransactionsService.Verify(x => x.DepositFundsAsync(
                "ACC001", 100, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TransferFunds_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            var requestDto = new TransferFundsRequestDto
            {
                SenderAccountNumber = "", // Invalid
                Amount = -100 // Invalid
            };

            _controller.ModelState.AddModelError("SenderAccountNumber", "Required");
            _controller.ModelState.AddModelError("Amount", "Must be positive");

            // Act
            var result = await _controller.TransferFunds(requestDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockTransactionsService.Verify(x => x.TransferFundsAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), 
                It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllTransactions_ReturnsEmptyList_WhenNoTransactions()
        {
            // Arrange
            _mockTransactionsService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>());

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<TransactionResponseDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<TransactionResponseDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }
    }
}