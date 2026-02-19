using Bank_back.entities;
using Bank_back.Entities;
using Bank_back.repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Globalization;
using Transaction = Bank_back.entities.Transaction;

namespace Bank_back.Services
{
    public class AccountService
    {
        private readonly AccountRepository accountRepository;
        private readonly TransactionRepository transactionRepository;
        private readonly UserRepository userRepository;
        DateTime utcDate = DateTime.UtcNow;
        private readonly ICurrentUserService currentUserService;


        public AccountService(AccountRepository accountRepository, TransactionRepository transactionRepository, UserRepository userRepository, ICurrentUserService currentUserService)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.userRepository = userRepository;
            this.currentUserService = currentUserService;
        }

        public double PerformDeposit(int to_id, double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit must be greater than 0");
            }
            if (!accountRepository.BelongsToId(currentUserService.GetUserId(), to_id))
            {
                throw new UnauthorizedAccessException("You do not have permission to deposit to this account.");
            }

            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");
            connection.Open();
            using var dbTransaction = connection.BeginTransaction();

            try
            {
                double newBalance = accountRepository.UpdateBalance(to_id, amount, connection, dbTransaction);

                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                int from_id = to_id;

                var type = TransactionType.Deposit;

                transactionRepository.SaveTransaction(amount, from_id, to_id, type, connection, dbTransaction);

                dbTransaction.Commit();

                return newBalance;
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        public double PerformWithdrawal(int from_id, double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal must be greater than 0");
            }
            if (!accountRepository.BelongsToId(currentUserService.GetUserId(), from_id))
            {
                throw new UnauthorizedAccessException("You do not have permission to withdrawal from this account.");
            }

            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");
            connection.Open();
            using var dbTransaction = connection.BeginTransaction();

            try
            {
                double newBalance = accountRepository.UpdateBalance(from_id, -amount, connection, dbTransaction);

                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                int to_id = from_id;

                var type = TransactionType.Withdrawal;

                transactionRepository.SaveTransaction(-amount, from_id, to_id, type, connection, dbTransaction);

                dbTransaction.Commit();

                return newBalance;
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        public Account AddAccount(int user_id)
        {
            if (!userRepository.ExistsByUserId(user_id))
            {
                throw new ArgumentException("User not found or does not exist");
            }
            return accountRepository.AddAccount(user_id);
        }

        public double CheckBalance(int id)
        {
            if (!accountRepository.BelongsToId(currentUserService.GetUserId(), id))
            {
                throw new UnauthorizedAccessException("You do not have permission to view this account.");
            }

            return accountRepository.CheckBalance(id);
        }

        public bool AccountExists(int id)
        {
            return accountRepository.ExistsByAccountId(id);
        }

        public bool BelongsById(int accountId)
        {
            return accountRepository.BelongsToId(currentUserService.GetUserId(), accountId);
        }
    }
}