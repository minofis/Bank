namespace Bank.API.DTOs.TransactionDTOs
{
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public string SenderAccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }

        public string Timestamp { get; set; }
    }
}