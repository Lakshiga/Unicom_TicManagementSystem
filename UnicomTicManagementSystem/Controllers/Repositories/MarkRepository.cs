using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Repositories
{
    public class MarkRepository
    {
        public async Task<DataTable> GetAllMarksAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
            SELECT
                m.Id AS Id,
                s.ReferenceId AS StudentID,
                s.Name AS StudentName,
                m.Subject,
                m.Exam,
                m.Score
            FROM Marks m
            JOIN Students s ON m.StudentID = s.Id
        ";

                using (var cmd = new SQLiteCommand(query, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }


        public async Task AddMarkAsync(Guid studentGuid, string subject, string exam, int score)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
            INSERT INTO Marks (Id, StudentId, Subject, Exam, Score, CreatedDate, ModifiedDate)
            VALUES (@Id, @StudentId, @Subject, @Exam, @Score, @CreatedDate, @ModifiedDate)";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    var id = Guid.NewGuid();
                    var now = DateTime.Now;

                    cmd.Parameters.AddWithValue("@Id", id.ToString());
                    cmd.Parameters.AddWithValue("@StudentId", studentGuid.ToString());
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Exam", exam);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@CreatedDate", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@ModifiedDate", now.ToString("yyyy-MM-dd HH:mm:ss"));

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<(int referenceId, string name)?> GetStudentReferenceIdAndNameByGuidAsync(Guid studentGuid)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT ReferenceId, Name FROM Students WHERE Id = @StudentGuid";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentGuid", studentGuid.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int referenceId = Convert.ToInt32(reader["ReferenceId"]);
                            string name = reader["Name"].ToString();
                            return (referenceId, name);
                        }
                    }
                }
            }
            return null;
        }



        public async Task DeleteMarkAsync(Guid markId)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "DELETE FROM Marks WHERE Id = @MarkID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MarkID", markId.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task UpdateMarkAsync(Guid markId, Guid studentGuid, string subject, string exam, int score)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
            UPDATE Marks 
            SET 
                StudentId = @StudentId, 
                Subject = @Subject, 
                Exam = @Exam, 
                Score = @Score, 
                ModifiedDate = @ModifiedDate
            WHERE Id = @Id";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    // ✅ Ensure all parameters in the SQL have corresponding parameters in the command
                    cmd.Parameters.AddWithValue("@StudentId", studentGuid.ToString());
                    cmd.Parameters.AddWithValue("@Subject", subject ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Exam", exam ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Id", markId.ToString());

                    // Debugging tip (optional): Uncomment to check values
                    // Console.WriteLine($"Update SQL Params: StudentId={studentGuid}, Subject={subject}, Exam={exam}, Score={score}, Id={markId}");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }



        public async Task<(Guid studentGuid, string studentName)> GetStudentByReferenceIdAsync(int referenceId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Students WHERE ReferenceId = @refId", conn))
                {
                    cmd.Parameters.AddWithValue("@refId", referenceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Guid id = Guid.Parse(reader["Id"].ToString());
                            string name = reader["Name"].ToString();

                            return (id, name);
                        }
                    }
                }
            }

            return (Guid.Empty, null);
        }




        public async Task<string> GetStudentNameAsync(int studentId)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT Name FROM Students WHERE ReferenceId = @StudentID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        public async Task<DataTable> GetSubjectsByStudentAsync(Guid studentGuid)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
            SELECT DISTINCT SubjectName 
            FROM Subjects 
            WHERE SectionId IN (
                SELECT SectionId FROM Students WHERE Id = @StudentID
            )";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentGuid.ToString());
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        public async Task<DataTable> GetExamsAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT ExamName FROM ManageExam ORDER BY CreatedDate DESC";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
