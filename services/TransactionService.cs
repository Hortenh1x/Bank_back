using Bank_back.entities;
using Bank_back.repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;
using Bank_back.services;
using Bank_back.Services;

namespace Bank_back.services
{
    public class TransactionService
    {
        private readonly AccountRepository accountRepository;
        private readonly TransactionRepository transactionRepository;
        private readonly ICurrentUserService currentUserService;

        public TransactionService(AccountRepository accountRepository, TransactionRepository transactionRepository, ICurrentUserService currentUserService)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.currentUserService = currentUserService;
        }

        public Transaction PerformTransaction(int to_id, double amount, int from_id)
        {
            if (!accountRepository.BelongsToId(currentUserService.GetUserId(), from_id))
            {
                throw new UnauthorizedAccessException("You do not have permission to make a transaction from this account.");
            }
            if (amount <= 0)
            {
                throw new ArgumentException("Transaction must be greater than 0");
            }
            using var connection = new SqliteConnection(@"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db");

            connection.Open();
            using var transaction = connection.BeginTransaction();

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
