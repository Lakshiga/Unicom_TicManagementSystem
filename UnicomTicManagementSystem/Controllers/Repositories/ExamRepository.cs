using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class ExamRepository : BaseRepository<Exam>
    {
        public override List<Exam> GetAll()
        {
            var exams = new List<Exam>();
            var sql = @"SELECT e.Id, e.SubjectId, e.ExamName, e.ReferenceId, e.CreatedDate, e.ModifiedDate,
                               s.SubjectName
                        FROM ManageExam e
                        LEFT JOIN Subjects s ON e.SubjectId = s.Id
                        ORDER BY e.ExamName";

            using (var reader = ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    var exam = Exam.CreateExam(
                            ParseString(reader["ExamName"]),
                            ParseGuid(reader["SubjectId"])
                        );

                    // ✅ Set other fields like Id and dates
                    typeof(Exam).GetProperty(nameof(Exam.Id))?.SetValue(exam, ParseGuid(reader["Id"]));
                    typeof(Exam).GetProperty(nameof(Exam.ReferenceId))?.SetValue(exam, ParseInt(reader["ReferenceId"]));
                    typeof(Exam).GetProperty(nameof(Exam.CreatedDate))?.SetValue(exam, ParseDateTime(reader["CreatedDate"]));
                    typeof(Exam).GetProperty(nameof(Exam.ModifiedDate))?.SetValue(exam, ParseDateTime(reader["ModifiedDate"]));
                    typeof(Exam).GetProperty("SubjectName")?.SetValue(exam, ParseString(reader["SubjectName"]));
                    exams.Add(exam);
                }
            }

            return exams;
        }

        public override Exam GetById(Guid id)
        {
            var sql = @"SELECT e.Id, e.SubjectId, e.ExamName, e.ReferenceId, e.CreatedDate, e.ModifiedDate,
                               s.SubjectName
                        FROM ManageExam e
                        LEFT JOIN Subjects s ON e.SubjectId = s.Id
                        WHERE e.Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = ExecuteReader(sql, parameters))
            {
                if (reader.Read())
                {
                    return Exam.CreateExam(
                        ParseString(reader["ExamName"]),
                        ParseGuid(reader["SubjectId"])
                    );
                }
            }

            return null;
        }

        public override void Add(Exam entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Use reflection to set private properties
            typeof(Exam).GetProperty(nameof(Exam.Id))?.SetValue(entity, Guid.NewGuid());
            typeof(Exam).GetProperty(nameof(Exam.CreatedDate))?.SetValue(entity, DateTime.UtcNow);
            typeof(Exam).GetProperty(nameof(Exam.ModifiedDate))?.SetValue(entity, DateTime.UtcNow);

            var sql = @"INSERT INTO ManageExam (Id, SubjectId, ExamName, ReferenceId, CreatedDate, ModifiedDate) 
                       VALUES (@Id, @SubjectId, @ExamName, @ReferenceId, @CreatedDate, @ModifiedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@SubjectId", entity.SubjectId.ToString() },
                { "@ExamName", entity.ExamName },
                { "@ReferenceId", entity.ReferenceId },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public override void Update(Exam entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Use reflection to set the private property ModifiedDate
            typeof(Exam).GetProperty(nameof(Exam.ModifiedDate), BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(entity, DateTime.UtcNow);

            var sql = @"UPDATE ManageExam 
                       SET SubjectId = @SubjectId, ExamName = @ExamName, 
                           ReferenceId = @ReferenceId, ModifiedDate = @ModifiedDate 
                       WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@SubjectId", entity.SubjectId.ToString() },
                { "@ExamName", entity.ExamName },
                { "@ReferenceId", entity.ReferenceId },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            ExecuteNonQuery(sql, parameters);
        }

        public override void Delete(Guid id)
        {
            var sql = "DELETE FROM ManageExam WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            ExecuteNonQuery(sql, parameters);
        }

        public List<Exam> GetExamsBySubject(Guid subjectId)
        {
            var exams = new List<Exam>();
            var sql = @"SELECT e.Id, e.SubjectId, e.ExamName, e.ReferenceId, e.CreatedDate, e.ModifiedDate
                FROM ManageExam e
                WHERE e.SubjectId = @SubjectId
                ORDER BY e.ExamName";

            var parameters = new Dictionary<string, object> { { "@SubjectId", subjectId.ToString() } };

            using (var reader = ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    var exam = Exam.CreateExam(
                        ParseString(reader["ExamName"]),
                        ParseGuid(reader["SubjectId"])
                    );

                    // Set other properties using reflection since they are private set
                    typeof(Exam).GetProperty(nameof(Exam.Id))?.SetValue(exam, ParseGuid(reader["Id"]));
                    typeof(Exam).GetProperty(nameof(Exam.ReferenceId))?.SetValue(exam, ParseInt(reader["ReferenceId"]));
                    typeof(Exam).GetProperty(nameof(Exam.CreatedDate))?.SetValue(exam, ParseDateTime(reader["CreatedDate"]));
                    typeof(Exam).GetProperty(nameof(Exam.ModifiedDate))?.SetValue(exam, ParseDateTime(reader["ModifiedDate"]));

                    exams.Add(exam);
                }
            }

            return exams;
        }

        public Exam GetByName(string examName)
        {
            var sql = "SELECT Id, SubjectId, ExamName, ReferenceId, CreatedDate, ModifiedDate FROM ManageExam WHERE ExamName = @ExamName";
            var parameters = new Dictionary<string, object> { { "@ExamName", examName } };

            using (var reader = ExecuteReader(sql, parameters))
            {
                if (reader.Read())
                {
                    return Exam.CreateExam(
                        ParseString(reader["ExamName"]),
                        ParseGuid(reader["SubjectId"])
                    );
                }
            }

            return null;
        }
    }
}
