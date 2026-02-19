using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.Entities;
using Bank_back.entities;
using Microsoft.Data.Sqlite;

namespace Bank_back.repositories
{
    public class UserRepository
    {
        string connectionString = @"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db";

        public UserRepository() { }

        public User FindUserById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT u.id, u.first_name, u.last_name FROM User u WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_first_name = reader.GetString(1);
                    var read_last_name = reader.GetString(2);

                    return new User(read_id, read_first_name, read_last_name);
                }
                else
                {
                    throw new KeyNotFoundException($"User with id {id} not found");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding user: {ex.Message}", ex);
            }
        }
        public int[] FindUsersAccounts(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT a.id FROM Account a WHERE a.user_id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                var ids = new List<int>();
                while (reader.Read())
                {
                    ids.Add(reader.GetInt32(0));
                }

                if (ids.Count == 0)
                {
                    throw new KeyNotFoundException($"Accounts of user with id {id} not found");
                }

                return ids.ToArray();
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding users accounts: {ex.Message}", ex);
            }
        }

        public Account[] GetAccountsByUserId(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT a.id, a.deposit, a.user_id FROM Account a WHERE a.user_id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                var accs = new List<Account>();
                while (reader.Read())
                {
                    var acc = new Account(reader.GetInt32(0), reader.GetDouble(1), reader.GetInt32(2));
                    accs.Add(acc);
                }
                if (accs.Count == 0)
                {
                    throw new KeyNotFoundException($"Accounts of user with id {id} not found");
                }

                return accs.ToArray();
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while finding account: {ex.Message}", ex);
            }
        }

        public bool ExistsByUserId(int id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT u.id FROM User u WHERE id = @id";
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
                throw new InvalidOperationException($"Database error while checking user existence: {ex.Message}", ex);
            }
        }

        public User SaveUser(string first_name, string last_name, string password_hash)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO User (first_name, last_name, password_hash) VALUES (@first_name, @last_name, @password_hash) RETURNING id, first_name, last_name, password_hash;";
                insertCmd.Parameters.AddWithValue("@first_name", first_name);
                insertCmd.Parameters.AddWithValue("@last_name", last_name);
                insertCmd.Parameters.AddWithValue("@password_hash", password_hash);

                using var reader = insertCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_id = reader.GetInt32(0);
                    var read_first_name = reader.GetString(1);
                    var read_last_name = reader.GetString(2);
                    var read_password_hash = reader.GetString(3);

                    return new User(read_id, read_first_name, read_last_name, read_password_hash);
                }
                else
                {
                    throw new KeyNotFoundException("Failed to create the user");
                }
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while creating user: {ex.Message}", ex);
            }
        }
        public bool CheckPassword(int id, string password)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT u.password_hash FROM User u WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    var read_password = reader.GetString(0);
                    if (read_password == password)
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
                throw new InvalidOperationException($"Database error while checking user existence: {ex.Message}", ex);
            }
        }
        public string GetHashedPassword(int user_id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT u.password_hash FROM User u WHERE id = @id";
                selectCmd.Parameters.AddWithValue("@id", user_id);

                using var reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetString(0);

                }

                throw new KeyNotFoundException("Failed to get users password");

            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException($"Database error while checking user existence: {ex.Message}", ex);
            }
        }
    }
}
