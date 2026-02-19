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

        public User RegisterUser(string first_name, string last_name, string password_hash)
        {
            // if (userRepository.ExistsByUserId(id))
            // {
            //     throw new ArgumentException("User with this id already exists");
            // }

            // User user = new User(id, first_name, last_name, password_hash);
            return userRepository.SaveUser(first_name, last_name, password_hash);
        }
        // public int GetUserId()
        // {
        //     return currentUserService.GetUserId();
        // }

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
    }
}
