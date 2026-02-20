using Bank_back.entities;
using Bank_back.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.repositories;

namespace Bank_back.repositories
{
    public class AccountRepository
    {
        private readonly string connectionString;
        private readonly UserRepository userRepository;

        public AccountRepository(IConfiguration configuration, UserRepository userRepository)
        {
            connectionString = Bank_back.utils.DatabaseConnection.ResolveConnectionString(configuration);
            this.userRepository = userRepository;
        }

        public Account FindAccountById(int id)
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

        public bool ExistsById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT a.id FROM Account a WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    if (read_id == id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while checking account existence: {ex.Message}", ex);
            }
        }
        public double CheckBalance(int id)
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



        public double UpdateBalance(int id, double transfer, SqliteConnection connection, SqliteTransaction transaction)
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

        public bool ExistsByAccountId(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT 1 FROM Account WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                var result = selectCmd.ExecuteScalar();
                return result != null;
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while checking account existence: {ex.Message}", ex);
            }
        }

        public Account AddAccount(int user_id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO Account (deposit, user_id) VALUES (0, @user_id) RETURNING id, deposit, user_id;";
                insertCmd.Parameters.AddWithValue("@user_id", user_id);

                using var reader = insertCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_deposit = reader.GetDouble(1);
                    var read_user_id = reader.GetInt32(2);
                    return new Account(read_id, read_deposit, read_user_id);
                }
                else
                {
                    throw new KeyNotFoundException($"Account not found");
                }

            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while creating account: {ex.Message}", ex);
            }
        }
        public bool BelongsToId(int userId, int accountId)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                var selectCmd = connection.CreateCommand();

                selectCmd.CommandText = "SELECT 1 FROM Account WHERE id = @accountId AND user_id = @userId LIMIT 1";
                selectCmd.Parameters.AddWithValue("@accountId", accountId);
                selectCmd.Parameters.AddWithValue("@userId", userId);

                var result = selectCmd.ExecuteScalar();

                return result != null;
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while verifying ownership: {ex.Message}", ex);
            }
        }
        public TransactionResponce[] ShowAccountsTransactions(int account_id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT t.id, t.date_time, t.deposit, t.from_id, t.to_id, t.type FROM [Transaction] t WHERE from_id = @account_id OR to_id = @account_id ORDER BY t.date_time DESC";
                selectCmd.Parameters.AddWithValue("@account_id", account_id);

                using var reader = selectCmd.ExecuteReader();

                var transactions = new List<TransactionResponce>();
                while (reader.Read())
                {
                    var transaction = new Transaction(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetInt32(3), reader.GetInt32(4), (TransactionType)Enum.Parse(typeof(TransactionType), reader.GetString(5)));
                    var from_owner = userRepository.FindUserById(FindAccountById(transaction.From_id).User_id).First_name + " " + userRepository.FindUserById(FindAccountById(transaction.From_id).User_id).Last_name;
                    var to_owner = userRepository.FindUserById(FindAccountById(transaction.To_id).User_id).First_name + " " + userRepository.FindUserById(FindAccountById(transaction.To_id).User_id).Last_name;
                    var transactionResponce = new TransactionResponce(transaction.Id, transaction.Date_time, transaction.Deposit, transaction.From_id, transaction.To_id, transaction.Type, from_owner, to_owner);
                    transactions.Add(transactionResponce);
                }
                if (transactions.Count == 0)
                {
                    throw new KeyNotFoundException($"Transactions of account with id {account_id} not found");
                }

                return transactions.ToArray();

            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding transaction: {ex.Message}", ex);
            }
        }
    }
}
