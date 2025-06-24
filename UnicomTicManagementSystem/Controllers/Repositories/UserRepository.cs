using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Services;

namespace UnicomTicManagementSystem.Repositories
{
    public static class UserRepository
    {
        public static async Task<int> AddUserAsync(User user, SQLiteTransaction transaction = null)
        {
            SQLiteConnection conn = null;
            bool shouldDispose = false;
            SQLiteTransaction localTransaction = null;
            
            try
            {
                if (transaction != null)
                {
                    conn = transaction.Connection;
                    localTransaction = transaction;
                }
                else
                {
                    conn = DbCon.GetConnection();
                    localTransaction = conn.BeginTransaction();
                    shouldDispose = true;
                }

                // Check if the username already exists
                var checkCmd = new SQLiteCommand(conn);
                checkCmd.Transaction = localTransaction;
                checkCmd.CommandText = "SELECT ReferenceId FROM Users WHERE Username = @username";
                checkCmd.Parameters.AddWithValue("@username", user.Username);

                var existingReferenceId = await checkCmd.ExecuteScalarAsync();

                if (existingReferenceId != null && existingReferenceId != DBNull.Value)
                {
                    // User already exists, throw exception instead of returning existing ID
                    if (shouldDispose)
                    {
                        localTransaction.Rollback();
                    }
                    throw new Exception("Username already exists. Please choose another.");
                }

                // Get the next available ReferenceId
                var maxCmd = new SQLiteCommand(conn);
                maxCmd.Transaction = localTransaction;
                maxCmd.CommandText = "SELECT COALESCE(MAX(ReferenceId), 0) + 1 FROM Users";
                var nextReferenceId = Convert.ToInt32(await maxCmd.ExecuteScalarAsync());

                // Proceed with insert
                var cmd = new SQLiteCommand(conn);
                cmd.Transaction = localTransaction;
                cmd.CommandText = @"INSERT INTO Users (Id, Username, Password, Role, ReferenceId, CreatedDate, ModifiedDate, IsActive) 
                                    VALUES (@id, @username, @password, @role, @referenceId, @createdDate, @modifiedDate, @isActive)";
                cmd.Parameters.AddWithValue("@id", user.Id.ToString());
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role);
                cmd.Parameters.AddWithValue("@referenceId", nextReferenceId);
                cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@isActive", user.IsActive ? 1 : 0);

                await cmd.ExecuteNonQueryAsync();
                
                if (shouldDispose)
                {
                    localTransaction.Commit();
                }
                
                return nextReferenceId;
            }
            catch (Exception ex)
            {
                if (shouldDispose && localTransaction != null)
                {
                    localTransaction.Rollback();
                }
                throw new Exception($"Error in AddUserAsync: {ex.Message}", ex);
            }
            finally
            {
                if (shouldDispose && conn != null)
                {
                    conn.Dispose();
                }
            }
        }

        public static async Task<User> GetUserByIdAsync(int id, SQLiteTransaction transaction = null)
        {
            SQLiteConnection conn = null;
            bool shouldDispose = false;

            try
            {
                if (transaction != null)
                {
                    conn = transaction.Connection;
                }
                else
                {
                    conn = DbCon.GetConnection();
                    shouldDispose = true;
                }

                try
                {
                    var cmd = new SQLiteCommand(conn);
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }
                    cmd.CommandText = "SELECT * FROM Users WHERE ReferenceId = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                            Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : string.Empty,
                            Password = reader["Password"] != DBNull.Value ? reader["Password"].ToString() : string.Empty,
                            Role = reader["Role"] != DBNull.Value ? reader["Role"].ToString() : string.Empty,
                            ReferenceId = reader["ReferenceId"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceId"]) : -1,
                            CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.MinValue,
                            ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : DateTime.MinValue,
                            LastLoginDate = reader["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastLoginDate"]) : (DateTime?)null,
                            IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                    else
                    {
                        throw new Exception($"No user found with ReferenceId: {id}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in GetUserByIdAsync for ReferenceId {id}: {ex.Message}", ex);
                }
            }
            finally
            {
                if (shouldDispose && conn != null)
                {
                    conn.Dispose();
                }
            }
        }


        public static async Task<int> GetUserIdByUsernameAsync(string username, SQLiteTransaction transaction = null)
        {
            SQLiteConnection conn = null;
            bool shouldDispose = false;

            try
            {
                if (transaction != null)
                {
                    conn = transaction.Connection;
                }
                else
                {
                    conn = DbCon.GetConnection();
                    shouldDispose = true;
                }

                var cmd = new SQLiteCommand(conn);
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                cmd.CommandText = "SELECT ReferenceId FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);

                var result = await cmd.ExecuteScalarAsync();
                return result != null && int.TryParse(result.ToString(), out int refId) ? refId : -1;
            }
            finally
            {
                if (shouldDispose && conn != null)
                {
                    conn.Dispose();
                }
            }
        }


        public static async Task UpdateUserAsync(User user)
        {
            if (user == null || user.Id == Guid.Empty)
                throw new ArgumentException("Invalid user information provided.");

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Username and password cannot be empty.");

            using (var conn = DbCon.GetConnection())
            {
                using (var checkCmd = new SQLiteCommand(conn))
                {
                    // Check if the username already exists for another user
                    checkCmd.CommandText = @"
                            SELECT COUNT(*) FROM Users 
                            WHERE Username = @username AND Id != @id";
                    checkCmd.Parameters.AddWithValue("@username", user.Username);
                    checkCmd.Parameters.AddWithValue("@id", user.Id.ToString());

                    var existingCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                    if (existingCount > 0)
                    {
                        throw new Exception("Username already exists for another user.");
                    }
                }

                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        UPDATE Users 
                        SET 
                        Username = @username, 
                        Password = @password, 
                        ModifiedDate = @modifiedDate 
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@modifiedDate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@id", user.Id.ToString());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("No user found with the given ID to update.");
                    }
                }
            }
        }




        public static string connectionString = "Data Source=unicomtic.db;Version=3;";

        public static async Task<User> AuthenticateAsync(string username, string password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                            SELECT * FROM Users 
                            WHERE LOWER(Username) = LOWER(@Username) 
                            AND Password = @Password 
                            AND IsActive = 1
                        ";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username.Trim());
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString(),
                                ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"]),
                                LastLoginDate = reader["LastLoginDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastLoginDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }

            return null;
        }



        public static async Task<bool> UserExistsAsync(string username)
        {
            using (var conn = DbCon.GetConnection())
            {
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                long count = (long)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public static async Task<bool> HasAnyUsersAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT COUNT(*) FROM Users";
                long count = (long)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public static async Task<bool> ValidateUserAsync(string username, string password)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public static async Task<bool> ResetUserPasswordAsync(string username, string newPassword)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "UPDATE Users SET Password = @Password, ModifiedDate = @ModifiedDate WHERE Username = @Username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public static async Task<User> GetUserByGuidAsync(Guid id)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM Users WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                                Username = reader["Username"]?.ToString(),
                                Password = reader["Password"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                ReferenceId = reader["ReferenceId"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceId"]) : -1,
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.MinValue,
                                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : DateTime.MinValue,
                                LastLoginDate = reader["LastLoginDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastLoginDate"]) : (DateTime?)null,
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }

            return null;
        }



        public static async Task<bool> TestUserOperationsAsync()
        {
            try
            {
                using (var conn = DbCon.GetConnection())
                {
                    // Test 1: Check if Users table exists and has data
                    var cmd1 = new SQLiteCommand(conn);
                    cmd1.CommandText = "SELECT COUNT(*) FROM Users";
                    var count = Convert.ToInt32(await cmd1.ExecuteScalarAsync());
                    
                    // Test 2: Try to get a user by username
                    var cmd2 = new SQLiteCommand(conn);
                    cmd2.CommandText = "SELECT ReferenceId FROM Users WHERE Username = 'admin'";
                    var adminRefId = await cmd2.ExecuteScalarAsync();
                    
                    return count > 0 && adminRefId != null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TestUserOperationsAsync failed: {ex.Message}");
                return false;
            }
        }

        public static async Task UpdateLastLoginDateAsync(Guid userId, DateTime loginDate)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "UPDATE Users SET LastLoginDate = @lastLoginDate, ModifiedDate = @modifiedDate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@lastLoginDate", loginDate);
                    cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", userId.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task DeleteUserAsync(Guid userId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DELETE FROM Users WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", userId.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
