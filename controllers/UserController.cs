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
    public class UserController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly ICurrentUserService currentUserService;
        private readonly UserService userService;

        public UserController(AccountService accountService, ICurrentUserService currentUserService, UserService userService)
        {
            this.accountService = accountService;
            this.currentUserService = currentUserService;
            this.userService = userService;
        }

        [HttpGet("me")]
        public IActionResult ShowMe()
        {
            try
            {
                int userId = currentUserService.GetUserId();
                UserReturn user = userService.ShowMe(userId);
                var response = new UserResponse
                {
                    Id = user.Id,
                    First_name = user.First_name,
                    Last_name = user.Last_name,
                    Accounts = user.ToStringAccounts()
                };
                return Ok(response);
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
        [HttpPost("add-account")]
        public IActionResult AddAccount()
        {
            try
            {
                currentUserService.GetUserId();
                if (currentUserService.GetUserId() == 0)
                {
                    return BadRequest("Invalid user id");
                }
                Account account = accountService.AddAccount(currentUserService.GetUserId());
                return Ok(new { id = account.Id, deposit = account.Deposit, belongs_to = account.User_id });
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
        [HttpGet("show-my-accounts")]
        public IActionResult SchowMyAccounts()
        {
            try
            {
                var userId = currentUserService.GetUserId();
                if (userId <= 0)
                {
                    return BadRequest("Invalid user id");
                }

                Account[] accounts = userService.GetAccountsByUserId(userId);

                return Ok(accounts);
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
    }

    public sealed class UserResponse
    {
        public required int Id { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public required string Accounts { get; set; }
    }
}