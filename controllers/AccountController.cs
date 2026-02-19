using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank_back.Services;
using Bank_back.services;
using Bank_back.entities;


namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    internal class AccountController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly ICurrentUserService currentUserService;
        private readonly UserService userService;

        public AccountController(AccountService accountService, ICurrentUserService currentUserService, UserService userService)
        {
            this.accountService = accountService;
            this.currentUserService = currentUserService;
            this.userService = userService;
        }

        [HttpGet("show-balance")]
        public IActionResult GetBalance()
        {
            try
            {
                int userId = currentUserService.GetUserId();
                double balance = accountService.CheckBalance(userId);
                return Ok(new { currentBalance = balance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
                double newBalance = accountService.PerformWithdrawal(userId, request.Amount);
                return Ok(new { id = userId, currentBalance = newBalance });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("me")]
        public IActionResult ShowMe()
        {
            try
            {
                int userId = currentUserService.GetUserId();
                UserReturn user = userService.ShowMe(userId);
                var response = new AccountResponse
                {
                    Id = user.Id,
                    First_name = user.First_name,
                    Last_name = user.Last_name,
                    Accounts = user.ToStringAccounts()
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }
    }
    internal sealed class DepositRequest
    {
        public required double Amount { get; set; }
    }

    internal sealed class WithdrawalRequest
    {
        public required double Amount { get; set; }
    }

    internal sealed class AccountResponse
    {
        public required int Id { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public required string Accounts { get; set; }
    }
}