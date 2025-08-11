namespace Bank.API.DTOs.TransactionDTOs
{
    public class DepositFundsRequestDto
    {
        public string? AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}