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
        public async Task<ActionResult<List<AccountResponseDto>>> GetAllAccounts()
        {
            var accounts = await _accountsService.GetAllAsync();

            var accountResponseDtos = accounts.Select(account => new AccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                HolderName = account.HolderName,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt.ToString()
            }).ToList();

            return Ok(accountResponseDtos);
        }

        [HttpGet("by-number")]
        public async Task<ActionResult<List<AccountResponseDto>>> GetAccountByNumber(string accountNumber)
        {
            var account = await _accountsService.GetByNumberAsync(accountNumber);

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
        public async Task<ActionResult<List<AccountResponseDto>>> CreateAccount([FromBody] AccountRequestDto requestDto)
        {
            await _accountsService.CreateAsync(requestDto.HolderName, requestDto.InitialBalance);

            return Ok();
        }
    }
}