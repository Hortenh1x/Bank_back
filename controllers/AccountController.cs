using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Bank_back.Services;
using Bank_back.services;
using Bank_back.entities;
using Bank_back.Entities;


namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
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

        [HttpGet("show-balance/{accountId}")]
        public IActionResult GetBalance(int accountId)
        {
            if (accountId <= 0)
            {
                return BadRequest(new { message = "Please enter a correct account id" });
            }

            if (!accountService.BelongsById(accountId))
            {
                return BadRequest(new { message = "You can't view the account, that's not yours" });
            }

            try
            {
                int userId = currentUserService.GetUserId();
                double balance = accountService.CheckBalance(accountId);
                return Ok(new { currentBalance = balance });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("deposit-money")]
        public IActionResult DepositMoney([FromBody] DepositRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Deposit amount must be greater than 0" });
            }
            if (request.To_id <= 0)
            {
                return BadRequest(new { message = "You must enter a corret account id" });
            }
            if (!accountService.BelongsById(request.To_id))
            {
                return BadRequest(new { message = "You can't deposit money to the account, that's not yours" });
            }


            try
            {
                double newBalance = accountService.PerformDeposit(request.To_id, request.Amount);
                return Ok(new { id = request.To_id, currentBalance = newBalance });
            }

            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("withdraw-money")]
        public IActionResult WithdrawMoney([FromBody] WithdrawalRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { message = "Withdrawal amount must be greater than 0" });
            }
            if (request.From_id <= 0)
            {
                return BadRequest(new { message = "You must enter a corret account id" });
            }
            if (!accountService.BelongsById(request.From_id))
            {
                return BadRequest(new { message = "You can't withdraw money from the account, that's not yours" });
            }

            try
            {
                double newBalance = accountService.PerformWithdrawal(request.From_id, request.Amount);
                return Ok(new { id = request.From_id, currentBalance = newBalance });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("show-accounts-transactions/{accountId}")]
        public IActionResult SchowAccountsTransactions(int accountId)
        {

            if (accountId <= 0)
            {
                return BadRequest(new { message = "Please enter a correct account id" });
            }

            if (!accountService.BelongsById(accountId))
            {
                return BadRequest(new { message = "You can't view the account, that's not yours" });
            }


            try
            {

                TransactionResponce[] transactions = accountService.ShowAccountsTransactions(accountId);

                return Ok(transactions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        public sealed class DepositRequest
        {
            public int To_id { get; set; }
            [Range(0.01, 1E+9, ErrorMessage = "Amount must be greater than and lower than 1 Billion.")]
            public double Amount { get; set; }
        }

        public sealed class WithdrawalRequest
        {
            public int From_id { get; set; }
            [Range(0.01, 1E+9, ErrorMessage = "Amount must be greater than and lower than 1 Billion.")]
            public double Amount { get; set; }
        }



        public sealed class CheckRequest
        {
            public int AccountId { get; set; }
        }
    }
}