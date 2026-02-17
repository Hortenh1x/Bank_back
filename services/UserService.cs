using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_business.entities;
using Bank_business.repositories;

namespace Bank_business.services
{
    internal class UserService
    {
        private readonly UserRepository userRepository;

        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
    }
}