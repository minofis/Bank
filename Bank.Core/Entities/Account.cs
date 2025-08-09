namespace Bank.Core.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Number { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
    }
}