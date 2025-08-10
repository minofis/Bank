namespace Bank.API.DTOs.TransactionDTOs
{
    public class WithdrawFundsRequestDto
    {
        public string? SenderAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}