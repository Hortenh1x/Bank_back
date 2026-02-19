using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.entities;
using Bank_back.Entities;
using Bank_back.repositories;
using Bank_back.Services;
using Bank_back.utils;

namespace Bank_back.services
{
    public class UserService
    {
        private readonly UserRepository userRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IConfiguration _configuration;

        public UserService(UserRepository userRepository, ICurrentUserService currentUserService, IConfiguration _configuration)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }

        public User RegisterUser(string first_name, string last_name, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password must be presented");
            }
            var pepper = _configuration["ApplicationSettings:Password_Pepper"];

            if (string.IsNullOrEmpty(pepper))
            {
                throw new InvalidOperationException("Password pepper is missing from configuration.");
            }
            var password_hash = PasswordHasher.HashPassword(password, pepper);

            return userRepository.SaveUser(first_name, last_name, password_hash);
        }

        public UserReturn ShowMe(int id)
        {
            if (userRepository.ExistsByUserId(id))
            {
                User user = userRepository.FindUserById(id);
                UserReturn me = new UserReturn(user.Id, user.First_name, user.Last_name, userRepository.FindUsersAccounts(id));
                return me;
            }
            throw new KeyNotFoundException("User with this id does not exist");
        }

        public Account[] GetAccountsByUserId(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid id");
            }
            if (userRepository.ExistsByUserId(userId))
            {
                return userRepository.GetAccountsByUserId(userId);
            }
            throw new KeyNotFoundException("User with this id not found");
        }
    }
}
