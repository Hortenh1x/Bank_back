using Bank_back.entities;
using Bank_back.repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Globalization;
using Transaction = Bank_back.entities.Transaction;

namespace Bank_back.Services
{
    internal class AccountService
    {
        private readonly AccountRepository accountRepository;
        private readonly TransactionRepository transactionRepository;
        private readonly UserRepository userRepository;
        DateTime utcDate = DateTime.UtcNow;

        public AccountService(AccountRepository accountRepository, TransactionRepository transactionRepository, UserRepository userRepository)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.userRepository = userRepository;
        }

        public double performDeposit(int to_id, double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit must be greater than 0");
            }

            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");
            connection.Open();
            using var dbTransaction = connection.BeginTransaction();

            try
            {
                double newBalance = accountRepository.updateBalance(to_id, amount, connection, dbTransaction);

                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                int from_id = userRepository.getUserId();

                var type = TransactionType.Deposit;

                transactionRepository.saveTransaction(amount, from_id, to_id, type, connection, dbTransaction);

                dbTransaction.Commit();

                return newBalance;
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        public double performWithdrawal(int to_id, double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal must be greater than 0");
            }

            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");
            connection.Open();
            using var dbTransaction = connection.BeginTransaction();

            try
            {
                double newBalance = accountRepository.updateBalance(to_id, -amount, connection, dbTransaction);

                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                int from_id = userRepository.getUserId();

                var type = TransactionType.Withdrawal;

                transactionRepository.saveTransaction(-amount, from_id, to_id, type, connection, dbTransaction);

                dbTransaction.Commit();

                return newBalance;
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        public double checkBalance(int id)
        {
            return accountRepository.checkBalance(id);
        }
    }
}
