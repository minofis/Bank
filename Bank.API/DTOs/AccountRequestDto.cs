namespace Bank.API.DTOs
{
    public class AccountRequestDto
    {
        public string HolderName { get; set; }
        public decimal InitialBalance { get; set; }
    }
}