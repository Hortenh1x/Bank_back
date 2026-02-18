using Bank_business.entities;
using Bank_business.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_business.repositories
{
    internal class AccountRepository
    {
        string connectionString = @"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db";

        public Account findAccountById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT a.id, a.deposit, a.user_id FROM Account a WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_deposit = reader.GetDouble(1);
                    var read_user_id = reader.GetInt32(2);

                    return new Account(read_id, read_deposit, read_user_id);
                }
                else
                {
                    throw new KeyNotFoundException($"Account with id {id} not found");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding account: {ex.Message}", ex);
            }
        }
        public double checkBalance(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT a.deposit FROM Account a WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetDouble(0);
                }
                else
                {
                    throw new KeyNotFoundException($"Account with id {id} not found");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding account: {ex.Message}", ex);
            }
        }

        public double updateBalance(int id, double transfer, SqliteConnection connection, SqliteTransaction transaction)
        {
            // 1. Get current balance using the shared connection
            var selectCmd = connection.CreateCommand();
            selectCmd.Transaction = transaction;
            selectCmd.CommandText = "SELECT deposit FROM Account WHERE id = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            var result = selectCmd.ExecuteScalar();
            if (result == null) throw new KeyNotFoundException($"Account {id} not found");

            double currentDeposit = Convert.ToDouble(result);
            double newDeposit = currentDeposit + transfer;

            // 2. Business Logic Check
            if (newDeposit < 0)
            {
                throw new ArgumentException("Insufficient funds");
            }

            // 3. Update the database using the shared connection
            var updateCmd = connection.CreateCommand();
            updateCmd.Transaction = transaction;
            updateCmd.CommandText = "UPDATE Account SET deposit = @read_deposit WHERE id = @id";
            updateCmd.Parameters.AddWithValue("@read_deposit", newDeposit);
            updateCmd.Parameters.AddWithValue("@id", id);

            updateCmd.ExecuteNonQuery();

            return newDeposit;
        }
    }
}