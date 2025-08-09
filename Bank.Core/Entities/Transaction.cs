using Bank.Core.Lookups;

namespace Bank.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int TypeId { get; set; }
        public TransactionType Type { get; set; } 

        public Guid SenderAccountId { get; set; }
        public Account SenderAccount { get; set; }

        public Guid RecipientAccountId { get; set; }
        public Account RecipientAccount { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}