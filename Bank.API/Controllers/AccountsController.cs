using Bank.API.DTOs;
using Bank.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("bank/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountResponseDto>>> GetAllAccounts(CancellationToken ct = default)
        {
            var accounts = await _accountsService.GetAllAsync(ct);

            var accountResponseDtos = accounts.Select(account => new AccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                HolderName = account.HolderName,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt.ToString("o")
            }).ToList();

            return Ok(accountResponseDtos);
        }

        [HttpGet("by-number")]
        public async Task<ActionResult<List<AccountResponseDto>>> GetAccountByNumber(string accountNumber, CancellationToken ct = default)
        {
            var account = await _accountsService.GetByNumberAsync(accountNumber, ct);

            var accountResponseDto = new AccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                HolderName = account.HolderName,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt.ToString()
            };

            return Ok(accountResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<List<AccountResponseDto>>> CreateAccount([FromBody] AccountRequestDto requestDto, CancellationToken ct = default)
        {
            var accountNumber = await _accountsService.CreateAsync(requestDto.HolderName, requestDto.InitialBalance, ct);

            return Ok(new {message = $"Account is created successfully with AccountNumber: {accountNumber}."});
        }
    }
}