using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace UnicomTicManagementSystem.Services
{
    internal class StudentTeacherService
    {
        public async Task AssignTeacherToStudentAsync(int studentId, int teacherId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        INSERT OR IGNORE INTO StudentTeacher (StudentId, TeacherId)
                        VALUES (@studentId, @teacherId)";
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task RemoveTeacherFromStudentAsync(int studentId, int teacherId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        DELETE FROM StudentTeacher 
                        WHERE StudentId = @studentId AND TeacherId = @teacherId";
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Teacher>> GetTeachersForStudentAsync(int studentId)
        {
            var teachers = new List<Teacher>();
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        SELECT T.Id, T.Name, T.Phone, T.Address
                        FROM Teachers T
                        INNER JOIN StudentTeacher ST ON T.Id = ST.TeacherId
                        WHERE ST.StudentId = @studentId";
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            teachers.Add(new Teacher
                            {
                                Id = Guid.Parse(reader.GetString(0)), // Fixed conversion from int to Guid
                                Name = reader.GetString(1),
                                Phone = reader.GetString(2),
                                Address = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return teachers;
        }
    }
}
