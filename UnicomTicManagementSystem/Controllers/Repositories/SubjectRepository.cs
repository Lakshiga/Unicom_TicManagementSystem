using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>
    {
        public override List<Subject> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public override async Task<List<Subject>> GetAllAsync()
        {
            var subjects = new List<Subject>();
            var sql = @"SELECT s.Id, s.SubjectName, s.SectionId, s.ReferenceId, s.CreatedDate, s.ModifiedDate,
                               sec.Name AS SectionName
                        FROM Subjects s
                        LEFT JOIN Sections sec ON s.SectionId = sec.Id
                        ORDER BY s.SubjectName";

            using (var reader = await ExecuteReaderAsync(sql))
            {
                while (await reader.ReadAsync())
                {
                    var subject = Subject.CreateSubject(
                        ParseString(reader["SubjectName"]),
                        ParseGuid(reader["SectionId"])
                    );

                    subject.Id = ParseGuid(reader["Id"]);
                    subject.ReferenceId = ParseInt(reader["ReferenceId"]);
                    subject.CreatedDate = ParseDateTime(reader["CreatedDate"]);
                    subject.ModifiedDate = ParseDateTime(reader["ModifiedDate"]);

                    subjects.Add(subject);
                }
            }

            return subjects;
        }

        public override Subject GetById(Guid id)
        {
            // Keep synchronous version for backward compatibility
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public override async Task<Subject> GetByIdAsync(Guid id)
        {
            var sql = @"SELECT s.Id, s.SubjectName, s.SectionId, s.ReferenceId, s.CreatedDate, s.ModifiedDate,
                               sec.Name AS SectionName
                        FROM Subjects s
                        LEFT JOIN Sections sec ON s.SectionId = sec.Id
                        WHERE s.Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    var subject = Subject.CreateSubject(
                        ParseString(reader["SubjectName"]),
                        ParseGuid(reader["SectionId"])
                    );

                    subject.Id = ParseGuid(reader["Id"]);
                    subject.ReferenceId = ParseInt(reader["ReferenceId"]);
                    subject.CreatedDate = ParseDateTime(reader["CreatedDate"]);
                    subject.ModifiedDate = ParseDateTime(reader["ModifiedDate"]);

                    return subject;
                }
            }

            return null;
        }

        public override void Add(Subject entity)
        {
            // Keep synchronous version for backward compatibility
            AddAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task AddAsync(Subject entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"INSERT INTO Subjects (Id, SubjectName, SectionId, ReferenceId, CreatedDate, ModifiedDate) 
                       VALUES (@Id, @SubjectName, @SectionId, @ReferenceId, @CreatedDate, @ModifiedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@SubjectName", entity.SubjectName },
                { "@SectionId", entity.SectionId.ToString() },
                { "@ReferenceId", entity.ReferenceId },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Update(Subject entity)
        {
            // Keep synchronous version for backward compatibility
            UpdateAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task UpdateAsync(Subject entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"UPDATE Subjects 
                       SET SubjectName = @SubjectName, SectionId = @SectionId, 
                           ReferenceId = @ReferenceId, ModifiedDate = @ModifiedDate 
                       WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@SubjectName", entity.SubjectName },
                { "@SectionId", entity.SectionId.ToString() },
                { "@ReferenceId", entity.ReferenceId },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Delete(Guid id)
        {
            // Keep synchronous version for backward compatibility
            DeleteAsync(id).GetAwaiter().GetResult();
        }

        public override async Task DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM Subjects WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public List<Subject> GetSubjectsBySection(Guid sectionId)
        {
            // Keep synchronous version for backward compatibility
            return GetSubjectsBySectionAsync(sectionId).GetAwaiter().GetResult();
        }

        public async Task<List<Subject>> GetSubjectsBySectionAsync(Guid sectionId)
        {
            var subjects = new List<Subject>();
            var sql = @"SELECT s.Id, s.SubjectName, s.SectionId, s.ReferenceId, s.CreatedDate, s.ModifiedDate
                        FROM Subjects s
                        WHERE s.SectionId = @SectionId
                        ORDER BY s.SubjectName";

            var parameters = new Dictionary<string, object> { { "@SectionId", sectionId.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    var subject = Subject.CreateSubject(
                        ParseString(reader["SubjectName"]),
                        ParseGuid(reader["SectionId"])
                    );

                    subject.Id = ParseGuid(reader["Id"]);
                    subject.ReferenceId = ParseInt(reader["ReferenceId"]);
                    subject.CreatedDate = ParseDateTime(reader["CreatedDate"]);
                    subject.ModifiedDate = ParseDateTime(reader["ModifiedDate"]);

                    subjects.Add(subject);
                }
            }

            return subjects;
        }

        public Subject GetByName(string subjectName)
        {
            // Keep synchronous version for backward compatibility
            return GetByNameAsync(subjectName).GetAwaiter().GetResult();
        }

        public async Task<Subject> GetByNameAsync(string subjectName)
        {
            var sql = "SELECT Id, SubjectName, SectionId, ReferenceId, CreatedDate, ModifiedDate FROM Subjects WHERE SubjectName = @SubjectName";
            var parameters = new Dictionary<string, object> { { "@SubjectName", subjectName } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    var subject = Subject.CreateSubject(
                        ParseString(reader["SubjectName"]),
                        ParseGuid(reader["SectionId"])
                    );

                    subject.Id = ParseGuid(reader["Id"]);
                    subject.ReferenceId = ParseInt(reader["ReferenceId"]);
                    subject.CreatedDate = ParseDateTime(reader["CreatedDate"]);
                    subject.ModifiedDate = ParseDateTime(reader["ModifiedDate"]);

                    return subject;
                }
            }

            return null;
        }
    }
}