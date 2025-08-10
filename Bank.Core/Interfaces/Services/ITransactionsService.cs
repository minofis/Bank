namespace Bank.Core.Interfaces.Services
{
    public interface ITransactionsService
    {
        Task TransferFundsAsync(string senderNumber, string recipientNumber, decimal amount, string description, CancellationToken ct);
        Task WithdrawFundsAsync(string senderNumber, decimal amount, CancellationToken ct);
        Task DepositFundsAsync(string recipientNumber, decimal amount, CancellationToken ct);
    }
}