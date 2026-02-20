using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bank_back.Services;
using Bank_back.services;
using Bank_back.entities;

namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService transactionService;
        private readonly AccountService accountService;

        public TransactionController(TransactionService transactionService, AccountService accountService)
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
        }

        [HttpPost("transfer-money")]
        public IActionResult TransferMoney([FromBody] TransferRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Transfer amount must be greater than 0" });
            }

            if (request.From_id <= 0)
            {
                return BadRequest(new { message = "You must provide a valid source account id" });
            }

            if (request.To_id <= 0)
            {
                return BadRequest(new { message = "You must provide a valid target account id" });
            }
            if (!accountService.BelongsById(request.From_id))
            {
                return BadRequest(new { message = "You can't make a transaction from the account, that's not yours" });
            }

            try
            {

                int targetId = request.To_id;
                double amount = request.Amount;

                if (targetId == request.From_id || accountService.BelongsById(targetId))
                {
                    return BadRequest(new { message = "You can't transfer money to yourself" });
                }

                if (!accountService.AccountExists(targetId))
                {
                    return NotFound(new { message = "Target account not found" });
                }

                Transaction transaction = transactionService.PerformTransaction(targetId, amount, request.From_id);
                var response = new TransferResponse
                {
                    TransactionId = transaction.Id,
                    DateTime = transaction.Date_time,
                    Amount = transaction.Deposit,
                    FromId = transaction.From_id,
                    ToId = transaction.To_id,
                    Type = transaction.Type.ToString()
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Account not found" });
            }
        }
    }

    public sealed class TransferRequest
    {
        public int To_id { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double Amount { get; set; }
        public int From_id { get; set; }
    }

    public sealed class TransferResponse
    {
        public int TransactionId { get; set; }
        public string DateTime { get; set; } = string.Empty;
        public double Amount { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public string Type { get; set; } = string.Empty;
        // public string To_owner { get; set; } = string.Empty;
        // public string From_owner { get; set; } = string.Empty;
    }
}
