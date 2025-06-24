using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Umbraco.Core.Models.Membership;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;
using MyUser = UnicomTicManagementSystem.Models.User;


namespace UnicomTicManagementSystem.Controllers
{
    public class StaffController
    {
        public async Task<List<Staff>> GetAllStaffAsync()
        {
            var staffList = new List<Staff>();

            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Staff", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            staffList.Add(new Staff
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"]?.ToString() ?? "",
                                Address = reader["Address"]?.ToString() ?? "",
                                Email = reader["Email"]?.ToString() ?? "",
                                ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                                UserId = Guid.Parse(reader["UserId"].ToString()),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"])
                            });
                        }
                    }
                }
            }

            return staffList;
        }

        public async Task AddStaffAsync(Staff staff, string username, string password)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int userReferenceId = await UserRepository.GetUserIdByUsernameAsync(username, transaction);

                        if (userReferenceId == -1)
                        {
                            userReferenceId = await UserRepository.AddUserAsync(new MyUser
                            {
                                Username = username,
                                Password = password,
                                Role = "staff"
                            }, transaction);
                        }

                        var user = await UserRepository.GetUserByIdAsync(userReferenceId, transaction);
                        if (user == null || user.Id == Guid.Empty)
                            throw new Exception("User not found after creation.");

                        staff.Id = staff.Id == Guid.Empty ? Guid.NewGuid() : staff.Id;
                        staff.UserId = user.Id;
                        staff.CreatedDate = DateTime.Now;
                        staff.ModifiedDate = DateTime.Now;
                        staff.ReferenceId = userReferenceId;

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = @"
                                    INSERT INTO Staff 
                                    (Id, Name, Address, Email, ReferenceId, UserId, CreatedDate, ModifiedDate)
                                    VALUES 
                                    (@id, @name, @address, @email, @referenceId, @userId, @createdDate, @modifiedDate)";

                            cmd.Parameters.AddWithValue("@id", staff.Id.ToString());
                            cmd.Parameters.AddWithValue("@name", staff.Name);
                            cmd.Parameters.AddWithValue("@address", staff.Address ?? "");
                            cmd.Parameters.AddWithValue("@email", staff.Email);
                            cmd.Parameters.AddWithValue("@referenceId", staff.ReferenceId);
                            cmd.Parameters.AddWithValue("@userId", staff.UserId.ToString());
                            cmd.Parameters.AddWithValue("@createdDate", staff.CreatedDate);
                            cmd.Parameters.AddWithValue("@modifiedDate", staff.ModifiedDate);

                            await cmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("AddStaffAsync failed: " + ex.Message, ex);
                    }
                }
            }
        }

        public async Task UpdateStaffAsync(Staff staff, string username, string password)
        {
            using (var conn = DbCon.GetConnection()) // connection already opened inside
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update user manually inside the transaction
                        using (var cmdUser = new SQLiteCommand(conn))
                        {
                            cmdUser.Transaction = transaction;
                            cmdUser.CommandText = @"
                                    UPDATE Users
                                    SET Username = @username, Password = @password, ModifiedDate = @modifiedDate
                                    WHERE Id = @id";

                            cmdUser.Parameters.AddWithValue("@username", username);
                            cmdUser.Parameters.AddWithValue("@password", password);
                            cmdUser.Parameters.AddWithValue("@modifiedDate", DateTime.UtcNow);
                            cmdUser.Parameters.AddWithValue("@id", staff.UserId.ToString());

                            await cmdUser.ExecuteNonQueryAsync();
                        }

                        // Update staff
                        staff.ModifiedDate = DateTime.Now;

                        using (var cmdStaff = new SQLiteCommand(conn))
                        {
                            cmdStaff.Transaction = transaction;
                            cmdStaff.CommandText = @"
                                    UPDATE Staff 
                                    SET Name = @name, Email = @email, Address = @address, ModifiedDate = @modifiedDate
                                    WHERE Id = @id";

                            cmdStaff.Parameters.AddWithValue("@name", staff.Name);
                            cmdStaff.Parameters.AddWithValue("@email", staff.Email);
                            cmdStaff.Parameters.AddWithValue("@address", staff.Address ?? "");
                            cmdStaff.Parameters.AddWithValue("@modifiedDate", staff.ModifiedDate);
                            cmdStaff.Parameters.AddWithValue("@id", staff.Id.ToString());

                            await cmdStaff.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("UpdateStaffAsync failed: " + ex.Message, ex);
                    }
                }
            }
        }



        public async Task DeleteStaffAsync(Guid id)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    Guid userId = Guid.Empty;

                    using (var cmdSelect = new SQLiteCommand(conn))
                    {
                        cmdSelect.Transaction = transaction;
                        cmdSelect.CommandText = "SELECT UserId FROM Staff WHERE Id = @id";
                        cmdSelect.Parameters.AddWithValue("@id", id.ToString());

                        var result = await cmdSelect.ExecuteScalarAsync();
                        if (result != null && Guid.TryParse(result.ToString(), out var parsedUserId))
                            userId = parsedUserId;
                    }

                    using (var cmdDelete = new SQLiteCommand(conn))
                    {
                        cmdDelete.Transaction = transaction;
                        cmdDelete.CommandText = "DELETE FROM Staff WHERE Id = @id";
                        cmdDelete.Parameters.AddWithValue("@id", id.ToString());
                        await cmdDelete.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();

                    if (userId != Guid.Empty)
                        await UserRepository.DeleteUserAsync(userId);
                }
            }
        }

        public async Task<Staff> GetStaffByIdAsync(Guid id)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Staff WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Staff
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"]?.ToString() ?? "",
                                Address = reader["Address"]?.ToString() ?? "",
                                Email = reader["Email"]?.ToString() ?? "",
                                ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                                UserId = Guid.Parse(reader["UserId"].ToString()),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"])
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
