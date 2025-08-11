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

        [HttpGet]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetAllTransactions(CancellationToken ct = default)
        {
            var transactions = await _transactionsService.GetAllAsync(ct);

            var transactionResponseDtos = transactions.Select(transaction => new TransactionResponseDto
            {
                Id = transaction.Id,
                Type = transaction.Type.Name,
                Amount = transaction.Amount,
                Description = transaction.Description,
                SenderAccountNumber = transaction.SenderAccountNumber,
                RecipientAccountNumber = transaction.RecipientAccountNumber,
                Timestamp = transaction.Timestamp.ToString("o")
            }).ToList();

            return Ok(transactionResponseDtos);
        }

        [HttpPost("transfer-funds")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsRequestDto requestDto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _transactionsService.TransferFundsAsync(
                requestDto.SenderAccountNumber,
                requestDto.RecipientAccountNumber,
                requestDto.Amount,
                requestDto.Description,
                ct);

            return Ok(new
            {
                message = $"Transfer completed: {requestDto.Amount}$ " +
                          $"from {requestDto.SenderAccountNumber} " +
                          $"to {requestDto.RecipientAccountNumber}"
            });
        }

        [HttpPost("withdraw-funds")]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawFundsRequestDto requestDto, CancellationToken ct = default)
        {
            await _transactionsService.WithdrawFundsAsync(
                requestDto.AccountNumber,
                requestDto.Amount,
                ct
            );

            return Ok(new
            {
                message = $"Withdraw completed: {requestDto.Amount}$ " +
                          $"from {requestDto.AccountNumber} "
            });
        }

        [HttpPost("deposit-funds")]
        public async Task<IActionResult> DepositFunds([FromBody] DepositFundsRequestDto requestDto, CancellationToken ct = default)
        {
            await _transactionsService.DepositFundsAsync(
                requestDto.AccountNumber,
                requestDto.Amount,
                ct);

            return Ok(new
            {
                message = $"Deposit completed: {requestDto.Amount}$ " +
                          $"to {requestDto.AccountNumber} "
            });
        }
    }
}