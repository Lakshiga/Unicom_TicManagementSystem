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
                string query = "DELETE FROM Marks WHERE MarkID = @MarkID";
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
            SET StudentId = @StudentId, Subject = @Subject, Exam = @Exam, Score = @Score, ModifiedDate = @ModifiedDate
            WHERE MarkID = @MarkID";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MarkID", markId);
                    cmd.Parameters.AddWithValue("@StudentId", studentGuid.ToString());
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Exam", exam);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<(Guid studentGuid, string studentName)> GetStudentByReferenceIdAsync(int referenceId)
        {
            using (var conn = DbCon.GetConnection())  // connection already opened here
            {
                string query = "SELECT Id, Name FROM Students WHERE ReferenceId = @ReferenceId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReferenceId", referenceId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (Guid.Parse(reader["Id"].ToString()), reader["Name"].ToString());
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
                string query = "SELECT DISTINCT Exam FROM Marks"; // or use Exams table if you have it

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
