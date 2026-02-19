using Bank_back.entities;
using Bank_back.repositories;
using Bank_back.utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bank_back.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly string jwtIssuer = "Bank";
        private readonly string jwtAudience = "Clients";
        private readonly int jwtExpireInMin = 30;

        public AuthService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public string Login(int id, string password)
        {
            var user = _userRepository.FindUserById(id) ?? throw new InvalidOperationException("User not found");
            var password_hash = _userRepository.GetHashedPassword(id);
            var pepper = _configuration["ApplicationSettings:Password_Pepper"];

            if (string.IsNullOrEmpty(pepper))
            {
                throw new InvalidOperationException("Password pepper is missing from configuration.");
            }

            if (!PasswordHasher.VerifyPassword(password, password_hash, pepper))
            {
                throw new ArgumentException("Wrong password!");
            }
            return GenerateToken(user);
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["ApplicationSettings:JWT_Secret"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret is missing from configuration.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtExpireInMin),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
