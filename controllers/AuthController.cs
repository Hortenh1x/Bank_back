using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank_back.Services;
using Bank_back.services;
using Bank_back.entities;
using Microsoft.AspNetCore.Identity.Data;

namespace Bank_back.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly UserService userService;

        public AuthController(AuthService authService, UserService userService)
        {
            this.authService = authService;
            this.userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest(new { message = "There is something wrong with your register request" });
            }

            if (string.IsNullOrWhiteSpace(registerRequest.First_name) ||
                string.IsNullOrWhiteSpace(registerRequest.Last_name) ||
                string.IsNullOrWhiteSpace(registerRequest.Password) ||
                string.IsNullOrWhiteSpace(registerRequest.Password_repeat))
            {
                return BadRequest(new { message = "Please fill all fields" });
            }

            if (!registerRequest.First_name.All(char.IsLetter) || !registerRequest.Last_name.All(char.IsLetter))
            {
                return BadRequest(new { message = "Your name must contain only Latin letters" });
            }
            if (registerRequest.Password.Length < 8 || registerRequest.Password.Contains(" "))
            {
                return BadRequest(new { message = "Password must be at least 8 charachters long and not contain any spaces" });
            }
            if (registerRequest.Password != registerRequest.Password_repeat)
            {
                return BadRequest(new { message = "Passwords do not match!" });
            }
            try
            {
                User user = userService.RegisterUser(registerRequest.First_name, registerRequest.Last_name, registerRequest.Password);
                return Ok(new { id = user.Id, first_name = user.First_name, user.Last_name, message = "You can now login" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || loginRequest.Id == 0 || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Please fill all spaces" });
            }
            try
            {
                var token = authService.Login(loginRequest.Id, loginRequest.Password);
                return Ok(new { token });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public sealed class RegisterRequest
    {
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public required string Password { get; set; }
        public required string Password_repeat { get; set; }
    }

    public sealed class LoginRequest
    {
        public required int Id { get; set; }
        public required string Password { get; set; }
    }
}
