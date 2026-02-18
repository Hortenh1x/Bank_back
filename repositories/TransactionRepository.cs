using Bank_business.entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_business.repositories
{
    internal class TransactionRepository
    {
        string connectionString = @"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db";

        public Transaction findTransactionById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT t.id, t.date_time, t.deposit, t.from_id, t.to_id FROM [Transaction] t WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_date_time = reader.GetString(1);
                    var read_deposit = reader.GetDouble(2);
                    var read_from_id = reader.GetInt32(3);
                    var read_to_id = reader.GetInt32(4);

                    return new Transaction(read_id, read_date_time, read_deposit, read_from_id, read_to_id);
                }
                else
                {
                    throw new KeyNotFoundException($"Transaction with id {id} not found");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding transaction: {ex.Message}", ex);
            }
        }

        public Transaction saveTransaction(Transaction transaction)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO [Transaction] (id, date_time, deposit, from_id, to_id) 
                    VALUES (@id, @date_time, @deposit, @from_id, @to_id) 
                    RETURNING id, date_time, deposit, from_id, to_id;";

                insertCmd.Parameters.AddWithValue("@id", transaction.Id);
                insertCmd.Parameters.AddWithValue("@date_time", transaction.Date_time);
                insertCmd.Parameters.AddWithValue("@deposit", transaction.Deposit);
                insertCmd.Parameters.AddWithValue("@from_id", transaction.From_id);
                insertCmd.Parameters.AddWithValue("@to_id", transaction.To_id);

                using var reader = insertCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_date_time = reader.GetString(1);
                    var read_deposit = reader.GetDouble(2);
                    var read_from_id = reader.GetInt32(3);
                    var read_to_id = reader.GetInt32(4);

                    return new Transaction(read_id, read_date_time, read_deposit, read_from_id, read_to_id);
                }
                else
                {
                    throw new KeyNotFoundException($"Failed to create transaction record");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while creating transaction: {ex.Message}", ex);
            }
        }
    }
}
