using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.entities;
using Bank_back.repositories;
using Bank_back.Services;

namespace Bank_back.services
{
    internal class UserService
    {
        private readonly UserRepository userRepository;
        private readonly ICurrentUserService currentUserService;

        public UserService(UserRepository userRepository, ICurrentUserService currentUserService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public User RegisterUser(int id, string first_name, string last_name, string password_hash)
        {
            if (userRepository.ExistsByUserId(id))
            {
                throw new ArgumentException("User with this id already exists");
            }

            User user = new User(id, first_name, last_name, password_hash);
            return userRepository.SaveUser(user);
        }
        public int GetUserId()
        {
            return currentUserService.GetUserId();
        }
    }
}
