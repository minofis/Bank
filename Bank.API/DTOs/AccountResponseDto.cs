namespace Bank.API.DTOs
{
    public class AccountResponseDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public string CreatedAt { get; set; }
    }
}
