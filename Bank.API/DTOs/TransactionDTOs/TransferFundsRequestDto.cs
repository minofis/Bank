namespace Bank.API.DTOs.TransactionDTOs
{
    public class TransferFundsRequestDto
    {
        public string? SenderAccountNumber { get; set; }
        public string? RecipientAccountNumber { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}