namespace Bank.API.DTOs.TransactionDTOs
{
    public class DepositFundsRequestDto
    {
        public string? RecipientAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}