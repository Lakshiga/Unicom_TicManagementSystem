using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class StudentRepository : BaseRepository<Student>
    {
        private const string TableName = "Students";

        public override List<Student> GetAll()
        {
            var students = new List<Student>();
            var sql = $@"
                SELECT s.Id, s.Name, s.Address, s.SectionId, s.SectionName, s.Stream,
                       s.ReferenceId, s.UserId, s.LastAttendanceDate, s.IsActive
                FROM {TableName} s
                ORDER BY s.Name";

            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    students.Add(MapReaderToStudent(reader));
                }
            }
            return students;
        }

        public override Student GetById(Guid id)
        {
            var sql = $@"
                SELECT s.Id, s.Name, s.Address, s.SectionId, s.SectionName, s.Stream,
                       s.ReferenceId, s.UserId, s.LastAttendanceDate, s.IsActive
                FROM {TableName} s
                WHERE s.Id = @Id";

            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = ExecuteReader(sql, parameters))
            {
                if (reader.Read())
                {
                    return MapReaderToStudent(reader);
                }
            }
            return null;
        }

        public override void Add(Student entity)
        {
            var sql = $@"
                INSERT INTO {TableName} (
                    Id, Name, Address, SectionId, SectionName, Stream,
                    ReferenceId, UserId, CreatedDate, ModifiedDate, LastAttendanceDate, IsActive
                ) VALUES (
                    @Id, @Name, @Address, @SectionId, @SectionName, @Stream,
                    @ReferenceId, @UserId, @CreatedDate, @ModifiedDate, @LastAttendanceDate, @IsActive
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@Address", entity.Address },
                { "@SectionId", entity.SectionId.ToString() },
                { "@SectionName", entity.SectionName ?? string.Empty },
                { "@Stream", entity.Stream ?? string.Empty },
                { "@ReferenceId", entity.ReferenceId },
                { "@UserId", entity.UserId.ToString() },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate },
                { "@LastAttendanceDate", entity.LastAttendanceDate.HasValue ? entity.LastAttendanceDate.Value : (object)DBNull.Value },
                { "@IsActive", entity.IsActive ? 1 : 0 }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public override void Update(Student entity)
        {
            var sql = $@"
                UPDATE {TableName}
                SET Name = @Name,
                    Address = @Address,
                    SectionId = @SectionId,
                    SectionName = @SectionName,
                    Stream = @Stream,
                    ModifiedDate = @ModifiedDate,
                    LastAttendanceDate = @LastAttendanceDate,
                    IsActive = @IsActive
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@Address", entity.Address },
                { "@SectionId", entity.SectionId.ToString() },
                { "@SectionName", entity.SectionName ?? string.Empty },
                { "@Stream", entity.Stream ?? string.Empty },
                { "@ModifiedDate", DateTime.Now },
                { "@LastAttendanceDate", entity.LastAttendanceDate.HasValue ? entity.LastAttendanceDate.Value : (object)DBNull.Value },
                { "@IsActive", entity.IsActive ? 1 : 0 }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public override void Delete(Guid id)
        {
            var sql = $"DELETE FROM {TableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };
            ExecuteNonQuery(sql, parameters);
        }

        public async Task<Student> GetStudentByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                var sql = $@"
                    SELECT s.Id, s.Name, s.Address, s.SectionId, s.SectionName, s.Stream,
                           s.ReferenceId, s.UserId, s.LastAttendanceDate, s.IsActive
                    FROM {TableName} s
                    INNER JOIN Users u ON s.UserId = u.Id
                    WHERE u.Username = @Username";

                var parameters = new Dictionary<string, object> { { "@Username", username } };

                using (var reader = ExecuteReader(sql, parameters))
                {
                    if (reader.Read())
                    {
                        return MapReaderToStudent(reader);
                    }
                }
                return null;
            });
        }

        private Student MapReaderToStudent(IDataReader reader)
        {
            return new Student
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                Name = reader["Name"].ToString(),
                Address = reader["Address"].ToString(),
                SectionId = Guid.Parse(reader["SectionId"].ToString()),
                SectionName = reader["SectionName"].ToString(),
                Stream = reader["Stream"].ToString(),
                ReferenceId = Convert.ToInt32(reader["ReferenceId"]),
                UserId = Guid.Parse(reader["UserId"].ToString()),
                LastAttendanceDate = reader["LastAttendanceDate"] != DBNull.Value ? DateTime.Parse(reader["LastAttendanceDate"].ToString()) : (DateTime?)null,
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1
            };
        }

        public async Task<List<string>> GetSubjectsBySectionNameAsync(string sectionName)
        {
            return await Task.Run(() =>
            {
                var subjects = new List<string>();
                var sql = @"
                    SELECT DISTINCT sub.SubjectName 
                    FROM Subjects sub
                    INNER JOIN Sections sec ON sub.SectionId = sec.Id
                    WHERE sec.Name = @SectionName";
                var parameters = new Dictionary<string, object> { { "@SectionName", sectionName } };

                using (var reader = ExecuteReader(sql, parameters))
                {
                    while (reader.Read())
                    {
                        subjects.Add(reader["SubjectName"].ToString());
                    }
                }
                return subjects;
            });
        }

        public async Task<List<TimeTable>> GetTimetableBySectionAsync(string sectionName)
        {
            return await Task.Run(() =>
            {
                var timetables = new List<TimeTable>();
                var sql = @"
                    SELECT tt.*
                    FROM TimeTable tt
                    INNER JOIN Subjects s ON tt.Subject = s.SubjectName
                    INNER JOIN Sections sec ON s.SectionId = sec.Id
                    WHERE sec.Name = @SectionName
                    ORDER BY tt.Date, tt.TimeSlot";
                var parameters = new Dictionary<string, object> { { "@SectionName", sectionName } };

                using (var reader = ExecuteReader(sql, parameters))
                {
                    while (reader.Read())
                    {
                        var timeTable = TimeTable.CreateTimeTable(
                            reader["Subject"].ToString(),
                            reader["TimeSlot"].ToString(),
                            reader["Room"].ToString(),
                            DateTime.Parse(reader["Date"].ToString())
                        );
                        timetables.Add(timeTable);
                    }
                }
                return timetables;
            });
        }

        public async Task<List<Mark>> GetExamMarksByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                var marks = new List<Mark>();
                var sql = @"
            SELECT m.*, e.ExamName AS ExamName, s.SubjectName AS SubjectName
            FROM Marks m
            INNER JOIN ManageExam e ON m.Exam = e.Id
            INNER JOIN Subjects s ON e.SubjectId = s.Id
            INNER JOIN Students st ON m.StudentId = st.Id
            INNER JOIN Users u ON st.UserId = u.Id
            WHERE u.Username = @Username
            ORDER BY e.CreatedDate DESC";
                var parameters = new Dictionary<string, object> { { "@Username", username } };

                using (var reader = ExecuteReader(sql, parameters))
                {
                    while (reader.Read())
                    {
                        var mark = Mark.CreateMark(
                            Guid.Parse(reader["StudentId"].ToString()),
                            reader["SubjectName"].ToString(),
                            reader["ExamName"].ToString(),
                            Convert.ToInt32(reader["Score"])
                        );
                        marks.Add(mark);
                    }
                }
                return marks;
            });
        }

        public async Task<List<Attendance>> GetAttendanceByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                var attendances = new List<Attendance>();
                var sql = @"
            SELECT a.*, s.SubjectName AS SubjectName
            FROM Attendance a
            INNER JOIN Students st ON a.StudentId = st.Id
            INNER JOIN Users u ON st.UserId = u.Id
            INNER JOIN Subjects s ON a.SubjectId = s.Id
            WHERE u.Username = @Username
            ORDER BY a.Date DESC";
                var parameters = new Dictionary<string, object> { { "@Username", username } };

                using (var reader = ExecuteReader(sql, parameters))
                {
                    while (reader.Read())
                    {
                        var attendance = Attendance.CreateAttendance(
                            Guid.Parse(reader["StudentId"].ToString()),
                            Guid.Parse(reader["SubjectId"].ToString()),
                            Convert.ToDateTime(reader["Date"]),
                            reader["Status"].ToString()
                        );
                        attendances.Add(attendance);
                    }
                }
                return attendances;
            });
        }

        public Student GetByReferenceId(string referenceId)
        {
            var sql = "SELECT * FROM Students WHERE ReferenceId = @ReferenceId";
            var parameters = new Dictionary<string, object>
            {
                { "@ReferenceId", referenceId }
            };

            using (var reader = ExecuteReader(sql, parameters))
            {
                if (reader.Read())
                {
                    return Student.CreateStudent(
                        ParseGuid(reader["Id"]),
                        ParseString(reader["Name"]),
                        ParseInt(reader["ReferenceId"])
                    );
                }
            }

            return null;
        }
    }
}