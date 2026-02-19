using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Bank_back.Services;
using Bank_back.services;
using Bank_back.entities;

namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    internal class TransactionController : ControllerBase
    {
        private readonly TransactionService transactionService;
        private readonly AccountService accountService;
        private readonly ICurrentUserService currentUserService;

        public TransactionController(TransactionService transactionService, AccountService accountService, ICurrentUserService currentUserService)
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
            this.currentUserService = currentUserService;
        }

        [HttpPost("transfer-money")]
        public IActionResult TransferMoney([FromBody] TransferRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Transfer amount must be greater than 0" });
            }

            if (request.Target <= 0)
            {
                return BadRequest(new { message = "You must provide a valid target account id" });
            }

            try
            {
                int userId = currentUserService.GetUserId();
                int targetId = request.Target;
                double amount = request.Amount;

                if (targetId == userId)
                {
                    return BadRequest(new { message = "You can't transfer money to yourself" });
                }

                if (!accountService.AccountExists(targetId))
                {
                    return NotFound(new { message = "Target account not found" });
                }

                Transaction transaction = transactionService.PerformTransaction(targetId, amount, userId);
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

    internal sealed class TransferRequest
    {
        public required double Amount { get; set; }
        public required int Target { get; set; }
    }

    internal sealed class TransferResponse
    {
        public int TransactionId { get; set; }
        public string DateTime { get; set; } = string.Empty;
        public double Amount { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public string Type { get; set; } = string.Empty;
    }


}
