using Bank_back.entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = Bank_back.entities.Transaction;

namespace Bank_back.repositories
{
    public class TransactionRepository
    {
        private readonly string connectionString;

        public TransactionRepository(IConfiguration configuration)
        {
            connectionString = Bank_back.utils.DatabaseConnection.ResolveConnectionString(configuration);
        }

        public Transaction FindTransactionById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT t.id, t.date_time, t.deposit, t.from_id, t.to_id, t.type FROM [Transaction] t WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_date_time = reader.GetString(1);
                    var read_deposit = reader.GetDouble(2);
                    var read_from_id = reader.GetInt32(3);
                    var read_to_id = reader.GetInt32(4);
                    var read_type = (TransactionType)Enum.Parse(typeof(TransactionType), reader.GetString(5));

                    return new Transaction(read_id, read_date_time, read_deposit, read_from_id, read_to_id, read_type);
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

        public Transaction SaveTransaction(double deposit, int from_id, int to_id, TransactionType type, SqliteConnection connection, SqliteTransaction dbTransaction)
        {
            try
            {
                var insertCmd = connection.CreateCommand();
                insertCmd.Transaction = dbTransaction;

                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                insertCmd.CommandText = @"
                    INSERT INTO [Transaction] (date_time, deposit, from_id, to_id, type) 
                    VALUES (@date_time, @deposit, @from_id, @to_id, @type)
                    RETURNING id, date_time, deposit, from_id, to_id, type;";

                insertCmd.Parameters.AddWithValue("@date_time", timestamp);
                insertCmd.Parameters.AddWithValue("@deposit", deposit);
                insertCmd.Parameters.AddWithValue("@from_id", from_id);
                insertCmd.Parameters.AddWithValue("@to_id", to_id);
                insertCmd.Parameters.AddWithValue("@type", type.ToString());

                using var reader = insertCmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Transaction(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDouble(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        (TransactionType)Enum.Parse(typeof(TransactionType), reader.GetString(5))
                    );
                }

                throw new InvalidOperationException("Failed to create transaction record.");
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}", ex);
            }
        }


    }
}
