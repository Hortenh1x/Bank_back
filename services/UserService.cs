using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.entities;
using Bank_back.repositories;
using Bank_back.controllers;

namespace Bank_back.services
{
    internal class UserService
    {
        private readonly UserRepository userRepository;
        private readonly AccountController accountController;

        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.accountController = accountController ?? throw new ArgumentNullException(nameof(accountController));
        }

        public User registerUser(int id, string first_name, string last_name, string password_hash)
        {
            if (userRepository.existsByUserId(id))
            {
                throw new ArgumentException("User with this id already exists");
            }

            User user = new User(id, first_name, last_name, password_hash);
            return userRepository.saveUser(user);
        }
        public int getUserId()
        {
            return accountController.getUserId();
        }
    }
}