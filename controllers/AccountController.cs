using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bank_back.Services;

namespace Bank_back.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    internal class AccountController : ControllerBase
    {
        private readonly AccountService accountService;

        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet("show-balance")]
        public IActionResult getBalance()
        {
            double balance = accountService.checkBalance(getUserId());
            return Ok(new { id = getUserId(), currentBalance = balance });
        }

        [NonAction]
        public int getUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User id not found in the session");
            }
            int userId = int.Parse(userIdClaim);
            return userId;
        }
    }
}