using System.Data;
using Bank.Core.Entities;
using Bank.Core.Enums;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Bank.BLL.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionsService> _logger;
        public TransactionsService(IUnitOfWork unitOfWork, ILogger<TransactionsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task TransferFundsAsync(string senderNumber, string recipientNumber, decimal amount, string description)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (amount <= 0)
                    throw new ArgumentException("Transfer amount must be positive.");

                if (senderNumber == recipientNumber)
                    throw new ArgumentException("Cannot transfer funds to the same account.");

                // Lock both accounts for the duration of transaction
                var sender = await _unitOfWork.Accounts.GetByNumberLockedAsync(senderNumber)
                    ?? throw new ArgumentException($"Sender account {senderNumber} not found.");

                var recipient = await _unitOfWork.Accounts.GetByNumberLockedAsync(recipientNumber)
                    ?? throw new ArgumentException($"Recipient account {recipientNumber} not found.");

                // Check sufficient funds with pessimistic locking
                if (sender.Balance < amount)
                    throw new ArgumentException("Insufficient funds.");

                // Perform transfer
                sender.Balance -= amount;
                recipient.Balance += amount;

                // Record transaction
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TypeId = (int)TransactionTypes.Transfer,
                    SenderAccountNumber = senderNumber,
                    RecipientAccountNumber = recipientNumber,
                    Timestamp = DateTime.UtcNow,
                    Amount = amount,
                    Description = description
                };

                await _unitOfWork.Transactions.AddAsync(transaction);

                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Transfer failed from {Sender} to {Recipient}", senderNumber, recipientNumber);
                throw;
            }
        }

        public async Task WithdrawFundsAsync(string accountNumber, decimal amount)
        {
            try
            {
                if (amount <= 0)
                    throw new ArgumentException("Withdraw amount must be positive.");

                var account = await _unitOfWork.Accounts.GetByNumberLockedAsync(accountNumber)
                    ?? throw new ArgumentException($"Account {accountNumber} not found or could not be locked");

                if (account.Balance < amount)
                    throw new ArgumentException("Insufficient funds.");

                account.Balance -= amount;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TypeId = (int)TransactionTypes.Withdrawal,
                    SenderAccountNumber = accountNumber,
                    Timestamp = DateTime.UtcNow,
                    Amount = amount
                };

                await _unitOfWork.Transactions.AddAsync(transaction);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Withdraw failed for {Sender}", accountNumber);
                throw;
            }
        }

        public async Task DepositFundsAsync(string accountNumber, decimal amount)
        {
            try
            {
                if (amount <= 0)
                    throw new ArgumentException("Deposit amount must be positive.");

                var account = await _unitOfWork.Accounts.GetByNumberLockedAsync(accountNumber)
                    ?? throw new ArgumentException($"Account {accountNumber} not found or could not be locked");

                account.Balance += amount;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TypeId = (int)TransactionTypes.Deposit,
                    RecipientAccountNumber = accountNumber,
                    Timestamp = DateTime.UtcNow,
                    Amount = amount
                };

                await _unitOfWork.Transactions.AddAsync(transaction);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Deposit failed for {Sender}", accountNumber);
                throw;
            }
        }
    }
}