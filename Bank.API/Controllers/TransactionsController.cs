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
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequestDto requestDto)
        {
            await _transactionsService.TransferFundsAsync(
                requestDto.SenderAccountNumber,
                requestDto.RecipientAccountNumber,
                requestDto.Amount,
                requestDto.Description);

            return Ok();
        }

        [HttpPost("withdraw-funds")]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawFundsRequestDto requestDto)
        {
            await _transactionsService.WithdrawFundsAsync(
                requestDto.SenderAccountNumber,
                requestDto.Amount
            );

            return Ok();
        }

        [HttpPost("deposit-funds")]
        public async Task<IActionResult> DepositFunds([FromBody] DepositFundsRequestDto requestDto)
        {
            await _transactionsService.DepositFundsAsync(
                requestDto.RecipientAccountNumber,
                requestDto.Amount);

            return Ok();
        }
    }
}