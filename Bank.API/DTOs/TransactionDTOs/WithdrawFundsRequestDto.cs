namespace Bank.API.DTOs.TransactionDTOs
{
    public class WithdrawFundsRequestDto
    {
        public string? AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}