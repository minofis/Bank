using Bank.API.DTOs.TransactionDTOs;
using Bank.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("bank/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        [HttpPost("transfer-funds")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequestDto requestDto, CancellationToken ct = default)
        {
            await _transactionsService.TransferFundsAsync(
                requestDto.SenderAccountNumber,
                requestDto.RecipientAccountNumber,
                requestDto.Amount,
                requestDto.Description,
                ct);

            return Ok();
        }

        [HttpPost("withdraw-funds")]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawFundsRequestDto requestDto, CancellationToken ct = default)
        {
            await _transactionsService.WithdrawFundsAsync(
                requestDto.SenderAccountNumber,
                requestDto.Amount,
                ct
            );

            return Ok();
        }

        [HttpPost("deposit-funds")]
        public async Task<IActionResult> DepositFunds([FromBody] DepositFundsRequestDto requestDto, CancellationToken ct = default)
        {
            await _transactionsService.DepositFundsAsync(
                requestDto.RecipientAccountNumber,
                requestDto.Amount,
                ct);

            return Ok();
        }
    }
}