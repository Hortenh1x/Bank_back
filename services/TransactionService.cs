using Bank_back.entities;
using Bank_back.repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;


namespace Bank_back.services
{
    internal class TransactionService
    {
        private readonly AccountRepository accountRepository;
        private readonly TransactionRepository transactionRepository;

        public TransactionService(AccountRepository accountRepository, TransactionRepository transactionRepository)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
        }

        public Transaction PerformTransaction(int to_id, double amount, int from_id)
        {
            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");

            connection.Open();
            using var transaction = connection.BeginTransaction();

            if (amount <= 0)
            {
                throw new ArgumentException("Transaction must be greater than 0");
            }

            try
            {
                accountRepository.UpdateBalance(to_id, amount, connection, transaction);
                accountRepository.UpdateBalance(from_id, -amount, connection, transaction);
                var type = TransactionType.Transfer;

                var savedResult = transactionRepository.SaveTransaction(amount, from_id, to_id, type, connection, transaction);

                transaction.Commit();

                return savedResult;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
