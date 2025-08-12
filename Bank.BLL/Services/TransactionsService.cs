using Bank.Core.Entities;
using Bank.Core.Enums;
using Bank.Core.Exceptions;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Repositories;
using Bank.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Bank.BLL.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionsService> _logger;
        private readonly ITransactionsRepository _transactionsRepo;
        public TransactionsService(
            IUnitOfWork unitOfWork,
            ILogger<TransactionsService> logger,
            ITransactionsRepository transactionsRepo)
        {
            _unitOfWork = unitOfWork;
            _transactionsRepo = transactionsRepo;
            _logger = logger;
        }

        public async Task<List<Transaction>> GetAllAsync(CancellationToken ct = default)
        {
            return await _transactionsRepo.GetAllAsync(ct);
        }
        
        private Transaction CreateTransaction(
            TransactionTypes type,
            string senderNumber,
            string recipientNumber,
            decimal amount,
            string description)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                TypeId = (int)type,
                SenderAccountNumber = senderNumber,
                RecipientAccountNumber = recipientNumber,
                Timestamp = DateTime.UtcNow,
                Amount = amount,
                Description = description ?? type.ToString()
            };
        }

        public async Task TransferFundsAsync(
            string senderNumber,
            string recipientNumber,
            decimal amount,
            string description,
            CancellationToken ct)
        {
            try
            {
                // Start DB transaction
                await _unitOfWork.BeginTransactionAsync(ct);

                if (string.IsNullOrEmpty(senderNumber) || string.IsNullOrEmpty(recipientNumber))
                    throw new ArgumentException("Account number cannot be empty.");

                if (amount <= 0)
                    throw new ArgumentException("Transfer amount must be positive.");

                if (senderNumber == recipientNumber)
                    throw new ArgumentException("Cannot transfer funds to the same account.");

                // Lock both accounts for the duration of transaction
                var sender = await _unitOfWork.Accounts.GetByNumberLockedAsync(senderNumber, ct)
                    ?? throw new NotFoundException($"Sender account {senderNumber} not found or could not be locked.");

                var recipient = await _unitOfWork.Accounts.GetByNumberLockedAsync(recipientNumber, ct)
                    ?? throw new NotFoundException($"Recipient account {recipientNumber} not found or could not be locked.");

                if (sender.Balance < amount)
                    throw new InsufficientFundsException("Insufficient funds.");

                // Perform transfer
                sender.Balance -= amount;
                recipient.Balance += amount;

                // Record transaction
                var transaction = CreateTransaction(
                    TransactionTypes.Transfer,
                    senderNumber,
                    recipientNumber,
                    amount,
                    description);

                await _unitOfWork.Transactions.AddAsync(transaction, ct);

                // Commit transaction to DB
                await _unitOfWork.CommitAsync(ct);
                _logger.LogInformation("Transfer completed: {Amount} from {Sender} to {Recipient}",
                    amount, senderNumber, recipientNumber);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                _logger.LogError(ex, "Transfer failed from {Sender} to {Recipient}", senderNumber, recipientNumber);
                throw;
            }
        }

        public async Task WithdrawFundsAsync(string accountNumber, decimal amount, CancellationToken ct)
        {
            try
            {
                // Start DB transaction
                await _unitOfWork.BeginTransactionAsync(ct);

                if (string.IsNullOrEmpty(accountNumber))
                    throw new ArgumentException("Account number cannot be empty.");

                if (amount <= 0)
                    throw new ArgumentException("Withdraw amount must be positive.");

                // Lock accounts for the duration of transaction
                var account = await _unitOfWork.Accounts.GetByNumberLockedAsync(accountNumber, ct)
                    ?? throw new NotFoundException($"Account {accountNumber} not found or could not be locked.");

                if (account.Balance < amount)
                    throw new InsufficientFundsException("Insufficient funds.");

                // Perform withdraw
                account.Balance -= amount;

                // Record transaction
                var transaction = CreateTransaction(
                    TransactionTypes.Withdrawal,
                    accountNumber,
                    null,
                    amount,
                    TransactionTypes.Withdrawal.ToString());

                await _unitOfWork.Transactions.AddAsync(transaction, ct);

                // Commit transaction to DB
                await _unitOfWork.CommitAsync(ct);
                _logger.LogInformation("Withdraw completed: {Amount} from {Account}", 
                    amount, accountNumber);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                _logger.LogError(ex, "Withdraw failed for {Account}", accountNumber);
                throw;
            }
        }

        public async Task DepositFundsAsync(string accountNumber, decimal amount, CancellationToken ct = default)
        {
            try
            {
                // Start DB transaction
                await _unitOfWork.BeginTransactionAsync(ct);

                if (string.IsNullOrEmpty(accountNumber))
                    throw new ArgumentException("Account number cannot be empty.");

                if (amount <= 0)
                    throw new ArgumentException("Deposit amount must be positive.");

                // Lock accounts for the duration of transaction
                var account = await _unitOfWork.Accounts.GetByNumberLockedAsync(accountNumber, ct)
                    ?? throw new NotFoundException($"Account {accountNumber} not found or could not be locked");

                // Perform withdraw
                account.Balance += amount;

                // Record transaction
                var transaction = CreateTransaction(
                    TransactionTypes.Deposit,
                    null,
                    accountNumber,
                    amount,
                    TransactionTypes.Deposit.ToString());

                await _unitOfWork.Transactions.AddAsync(transaction, ct);

                // Commit transaction to DB
                await _unitOfWork.CommitAsync(ct);
                _logger.LogInformation("Transfer completed: {Amount} to {Account}", 
                    amount, accountNumber);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                _logger.LogError(ex, "Deposit failed for {Account}", accountNumber);
                throw;
            }
        }
    }
}