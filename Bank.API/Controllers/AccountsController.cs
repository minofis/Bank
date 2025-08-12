using Bank.API.DTOs;
using Bank.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
            Summary = "Retrieve all accounts",
            Description = "Returns a list of all accounts in the system.")]
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
        [SwaggerOperation(
            Summary = "Retrieve account details",
            Description = "Fetches complete account information for the specified account number")]
        public async Task<ActionResult<AccountResponseDto>> GetAccountByNumber(string accountNumber, CancellationToken ct = default)
        {
            var account = await _accountsService.GetByNumberAsync(accountNumber, ct);

            if (account == null)
            {
                return NotFound($"Account with number {accountNumber} not found.");
            }

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
        [SwaggerOperation(
            Summary = "Create a new bank account",
            Description = "Creates a new account with the specified holder name and initial balance.")]
        public async Task<ActionResult<List<AccountResponseDto>>> CreateAccount([FromBody] AccountRequestDto requestDto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountNumber = await _accountsService.CreateAsync(requestDto.HolderName, requestDto.InitialBalance, ct);

            return Ok(new {message = $"Account is created successfully with AccountNumber: {accountNumber}."});
        }
    }
}