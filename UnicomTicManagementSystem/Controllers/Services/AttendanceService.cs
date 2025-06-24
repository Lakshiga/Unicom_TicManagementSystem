using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;


namespace UnicomTicManagementSystem.Services
{
    public class AttendanceService
    {
        private readonly StudentRepository _studentRepository = new StudentRepository();

        public async Task AddAttendanceAsync(Attendance attendance)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"
                    INSERT INTO Attendance (Id, StudentId, SubjectId, Date, Status, CreatedDate, ModifiedDate)
                    VALUES (@Id, @StudentId, @SubjectId, @Date, @Status, @CreatedDate, @ModifiedDate)";

                        cmd.Parameters.AddWithValue("@Id", attendance.Id.ToString());
                        cmd.Parameters.AddWithValue("@StudentId", attendance.StudentId.ToString());
                        cmd.Parameters.AddWithValue("@SubjectId", attendance.SubjectId.ToString());
                        cmd.Parameters.AddWithValue("@Date", attendance.Date.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Status", attendance.Status);
                        cmd.Parameters.AddWithValue("@CreatedDate", attendance.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@ModifiedDate", attendance.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                        await cmd.ExecuteNonQueryAsync();
                        transaction.Commit();
                    }
                }
            }
        }


        public async Task UpdateAttendanceAsync(string id, Attendance attendance)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                UPDATE Attendance
                SET StudentId = @StudentId, SubjectId = @SubjectId, Date = @Date, Status = @Status, ModifiedDate = @ModifiedDate
                WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@StudentId", attendance.StudentId.ToString());
                    cmd.Parameters.AddWithValue("@SubjectId", attendance.SubjectId.ToString());
                    cmd.Parameters.AddWithValue("@Date", attendance.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Status", attendance.Status);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task DeleteAttendanceAsync(string id)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("DELETE FROM Attendance WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<DataTable> GetAllAttendanceAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = @"
                            SELECT
                                A.Id,
                                A.Date,
                                A.StudentId,
                                S.Name AS StudentName,
                                A.SubjectId,
                                Sub.SubjectName,
                                A.Status
                            FROM Attendance A
                            JOIN Students S ON A.StudentId = S.Id
                            JOIN Subjects Sub ON A.SubjectId = Sub.Id";

                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        public async Task<DataTable> SearchAttendanceAsync(DateTime date, string studentId, string studentName, string subjectName, string status)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;

                    string query = @"
                        SELECT A.Id, A.Date, A.StudentId, S.Name AS StudentName,
                               Sub.SubjectName, A.Status, A.SubjectId
                        FROM Attendance A
                        JOIN Students S ON A.StudentId = S.Id
                        JOIN Subjects Sub ON A.SubjectId = Sub.Id
                        WHERE 1=1";

                    if (!string.IsNullOrWhiteSpace(studentId))
                    {
                        query += " AND A.StudentId LIKE @StudentId";
                        cmd.Parameters.AddWithValue("@StudentId", $"%{studentId}%");
                    }

                    if (!string.IsNullOrWhiteSpace(studentName))
                    {
                        query += " AND S.Name LIKE @StudentName";
                        cmd.Parameters.AddWithValue("@StudentName", $"%{studentName}%");
                    }

                    if (!string.IsNullOrWhiteSpace(subjectName))
                    {
                        query += " AND Sub.SubjectName LIKE @SubjectName";
                        cmd.Parameters.AddWithValue("@SubjectName", $"%{subjectName}%");
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        query += " AND A.Status LIKE @Status";
                        cmd.Parameters.AddWithValue("@Status", $"%{status}%");
                    }

                    query += " AND A.Date = @Date";
                    cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

                    cmd.CommandText = query;

                    var dt = new DataTable();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }

                    return dt;
                }
            }
        }

        public async Task<dynamic> GetStudentByIDAsync(string studentId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Students WHERE ReferenceId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new
                            {
                                StudentID = reader["ReferenceId"].ToString(),
                                StudentName = reader["Name"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<DataTable> GetAllSubjectsAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Subjects", conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public async Task<DataTable> GetSubjectsByStudentIDAsync(string studentId)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand(@"
                    SELECT s.Id AS SubjectId, s.SubjectName
                    FROM Students st
                    JOIN Sections sec ON st.SectionId = sec.Id
                    JOIN Subjects s ON s.SectionId = sec.Id
                    WHERE st.Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentId);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        public async Task<Student> GetStudentByReferenceIdAsync(string referenceId)
        {
            return await Task.Run(() => _studentRepository.GetByReferenceId(referenceId));
        }

        public async Task<Student> GetStudentByGuidAsync(string studentGuid)
        {
            using (var conn = DbCon.GetConnection())
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Students WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", studentGuid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Student
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                                Name = reader["Name"].ToString(),
                                Address = reader["Address"].ToString(),
                                SectionId = Guid.TryParse(reader["SectionId"].ToString(), out var sectionId) ? sectionId : Guid.Empty
                            };
                        }
                    }
                }
            }

            return null;
        }


    }
}