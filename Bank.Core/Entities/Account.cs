namespace Bank.Core.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public decimal Balance { get; set; }

        public List<Transaction> SentTransactions { get; set; } = new();
        public List<Transaction> RecivedTransactions { get; set; } = new();
    }
}