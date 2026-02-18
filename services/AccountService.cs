using Bank_business.entities;
using Bank_business.repositories;

namespace Bank_business.Services
{
    internal class AccountService
    {
        private readonly AccountRepository accountRepository;

        public AccountService(AccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        //public 
    }
}
