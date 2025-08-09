using Bank.Core.Lookups;

namespace Bank.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int TypeId { get; set; }
        public TransactionType Type { get; set; } 

        public string SenderAccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}