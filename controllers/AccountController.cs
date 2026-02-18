using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank_back.Services;

namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    internal class AccountController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly ICurrentUserService currentUserService;

        public AccountController(AccountService accountService, ICurrentUserService currentUserService)
        {
            this.accountService = accountService;
            this.currentUserService = currentUserService;
        }

        [HttpGet("show-balance")]
        public IActionResult GetBalance()
        {
            int userId = currentUserService.GetUserId();
            double balance = accountService.CheckBalance(userId);
            return Ok(new { id = userId, currentBalance = balance });
        }


        [HttpPost("deposit-money")]
        public IActionResult DepositMoney([FromBody] DepositRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Deposit amount must be greater than 0" });
            }

            try
            {
                int userId = currentUserService.GetUserId();
                double newBalance = accountService.PerformDeposit(userId, request.Amount);
                return Ok(new { id = userId, currentBalance = newBalance });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("withdraw-money")]
        public IActionResult WithdrawMoney([FromBody] WithdrawalRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Withdrawal amount must be greater than 0" });
            }

            try
            {
                int userId = currentUserService.GetUserId();
                double newBalance = accountService.PerformDeposit(userId, request.Amount);
                return Ok(new { id = userId, currentBalance = newBalance });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    internal sealed class DepositRequest
    {
        public double Amount { get; set; }
    }

    internal sealed class WithdrawalRequest
    {
        public double Amount { get; set; }
    }



}
