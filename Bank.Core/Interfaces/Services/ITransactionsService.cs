namespace Bank.Core.Interfaces.Services
{
    public interface ITransactionsService
    {
        Task TransferFundsAsync(string senderNumber, string recipientNumber, decimal amount, string description);
        Task WithdrawFundsAsync(string accountNumber, decimal amount);
        Task DepositFundsAsync(string accountNumber, decimal amount);
    }
}