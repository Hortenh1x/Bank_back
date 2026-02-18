using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_back.Entities;
using Bank_back.entities;
using Microsoft.Data.Sqlite;
using Bank_back.services;

namespace Bank_back.repositories
{
    internal class UserRepository
    {
        string connectionString = @"Data Source=C:\Users\Trainee1\source\repos\Bank_db\bank_db.db";
        private readonly UserService userService;

        public UserRepository(UserService userService)
        {
            this.userService = userService;
        }

        public User findUserById(int id)
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
        
        public bool existsByUserId(int id)
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

        public User saveUser(User user)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("connected");
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO User (id, first_name, last_name, password_hash) VALUES (@id, @first_name, @last_name, @password_hash) RETURNING id, first_name, last_name, password_hash;";
                insertCmd.Parameters.AddWithValue("@id", user.Id);
                insertCmd.Parameters.AddWithValue("@first_name", user.First_name);
                insertCmd.Parameters.AddWithValue("@last_name", user.Last_name);
                insertCmd.Parameters.AddWithValue("@password_hash", user.Password_hash);

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

        public int getUserId()
        {
            return userService.getUserId();
        }
    }
}