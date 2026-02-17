using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Bank_business.entities;
using Bank_business.repositories;

namespace Bank_business.Services
{
    internal class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly string jwtKey = "kdjguofkdjvgoldmgfidhglsjuekgvyhdsiensalifdsaj";
        private readonly string jwtIssuer = "Bank";
        private readonly string jwtAudience = "Clients";
        private readonly int jwtExpireInMin = 30;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
        }

    }
}
