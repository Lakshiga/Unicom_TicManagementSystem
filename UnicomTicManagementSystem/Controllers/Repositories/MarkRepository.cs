using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Data;

namespace UnicomTicManagementSystem.Repositories
{
    public class MarkRepository
    {
        public async Task<DataTable> GetAllMarksAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT * FROM Marks";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public async Task AddMarkAsync(int studentId, string subject, string exam, int score)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "INSERT INTO Marks (StudentID, Subject, Exam, Score) VALUES (@StudentID, @Subject, @Exam, @Score)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Exam", exam);
                    cmd.Parameters.AddWithValue("@Score", score);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteMarkAsync(int markId)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "DELETE FROM Marks WHERE MarkID = @MarkID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MarkID", markId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateMarkAsync(int markId, int studentId, string subject, string exam, int score)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "UPDATE Marks SET StudentID = @StudentID, Subject = @Subject, Exam = @Exam, Score = @Score WHERE MarkID = @MarkID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MarkID", markId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Exam", exam);
                    cmd.Parameters.AddWithValue("@Score", score);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
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

        public async Task<DataTable> GetSubjectsByStudentAsync(int studentId)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT DISTINCT SubjectName \r\nFROM Subjects \r\nWHERE SectionId IN (SELECT SectionId FROM Students WHERE Id = @StudentID)\r\n";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public async Task<DataTable> GetAllExamsAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT DISTINCT ExamName FROM ManageExam";
                using (var cmd = new SQLiteCommand(query, conn))
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
