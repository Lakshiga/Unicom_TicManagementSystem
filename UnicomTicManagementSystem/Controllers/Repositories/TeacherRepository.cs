using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>
    {
        public override List<Teacher> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public override async Task<List<Teacher>> GetAllAsync()
        {
            var teachers = new List<Teacher>();
            var sql = @"SELECT Id, Name, Phone, Address, ReferenceId, UserId, CreatedDate, ModifiedDate 
                        FROM Teachers ORDER BY Name";

            using (var reader = await ExecuteReaderAsync(sql))
            {
                while (await reader.ReadAsync())
                {
                    teachers.Add(new Teacher
                    {
                        Id = ParseGuid(reader["Id"]),
                        Name = ParseString(reader["Name"]),
                        Phone = ParseString(reader["Phone"]),
                        Address = ParseString(reader["Address"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        UserId = ParseGuid(reader["UserId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    });
                }
            }

            return teachers;
        }

        public override Teacher GetById(Guid id)
        {
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public override async Task<Teacher> GetByIdAsync(Guid id)
        {
            var sql = @"SELECT Id, Name, Phone, Address, ReferenceId, UserId, CreatedDate, ModifiedDate 
                        FROM Teachers WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    return new Teacher
                    {
                        Id = ParseGuid(reader["Id"]),
                        Name = ParseString(reader["Name"]),
                        Phone = ParseString(reader["Phone"]),
                        Address = ParseString(reader["Address"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        UserId = ParseGuid(reader["UserId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    };
                }
            }

            return null;
        }

        public override void Add(Teacher entity)
        {
            AddAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task AddAsync(Teacher entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"INSERT INTO Teachers 
                        (Id, Name, Phone, Address, ReferenceId, UserId, CreatedDate, ModifiedDate) 
                        VALUES 
                        (@Id, @Name, @Phone, @Address, @ReferenceId, @UserId, @CreatedDate, @ModifiedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@Phone", entity.Phone },
                { "@Address", entity.Address },
                { "@ReferenceId", entity.ReferenceId },
                { "@UserId", entity.UserId.ToString() },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Update(Teacher entity)
        {
            UpdateAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task UpdateAsync(Teacher entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"UPDATE Teachers 
                        SET Name = @Name, Phone = @Phone, Address = @Address, 
                            ReferenceId = @ReferenceId, UserId = @UserId, ModifiedDate = @ModifiedDate 
                        WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@Phone", entity.Phone },
                { "@Address", entity.Address },
                { "@ReferenceId", entity.ReferenceId },
                { "@UserId", entity.UserId.ToString() },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Delete(Guid id)
        {
            DeleteAsync(id).GetAwaiter().GetResult();
        }

        public override async Task DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM Teachers WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public List<Teacher> GetTeachersBySection(Guid sectionId)
        {
            return GetTeachersBySectionAsync(sectionId).GetAwaiter().GetResult();
        }

        public async Task<List<Teacher>> GetTeachersBySectionAsync(Guid sectionId)
        {
            var teachers = new List<Teacher>();
            var sql = @"SELECT DISTINCT t.Id, t.Name, t.Phone, t.Address, t.ReferenceId, t.UserId, t.CreatedDate, t.ModifiedDate 
                        FROM Teachers t 
                        INNER JOIN TeacherSection ts ON t.Id = ts.TeacherId 
                        WHERE ts.SectionId = @SectionId 
                        ORDER BY t.Name";

            var parameters = new Dictionary<string, object> { { "@SectionId", sectionId.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    teachers.Add(new Teacher
                    {
                        Id = ParseGuid(reader["Id"]),
                        Name = ParseString(reader["Name"]),
                        Phone = ParseString(reader["Phone"]),
                        Address = ParseString(reader["Address"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        UserId = ParseGuid(reader["UserId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    });
                }
            }

            return teachers;
        }

        public int AddWithReturnId(Teacher entity)
        {
            return AddWithReturnIdAsync(entity).GetAwaiter().GetResult();
        }

        public async Task<int> AddWithReturnIdAsync(Teacher entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"INSERT INTO Teachers 
                        (Id, Name, Phone, Address, ReferenceId, UserId, CreatedDate, ModifiedDate) 
                        VALUES 
                        (@Id, @Name, @Phone, @Address, @ReferenceId, @UserId, @CreatedDate, @ModifiedDate);
                        SELECT last_insert_rowid();";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@Phone", entity.Phone },
                { "@Address", entity.Address },
                { "@ReferenceId", entity.ReferenceId },
                { "@UserId", entity.UserId.ToString() },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            var result = await ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result);
        }

        public async Task AssignSectionToTeacherAsync(Guid teacherId, Guid sectionId)
        {
            var sql = @"INSERT OR REPLACE INTO TeacherSection 
                        (TeacherId, SectionId, CreatedDate, ModifiedDate) 
                        VALUES (@TeacherId, @SectionId, @CreatedDate, @ModifiedDate)";

            var now = DateTime.UtcNow;
            var parameters = new Dictionary<string, object>
            {
                { "@TeacherId", teacherId.ToString() },
                { "@SectionId", sectionId.ToString() },
                { "@CreatedDate", now },
                { "@ModifiedDate", now }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public List<Teacher> Search(string keyword)
        {
            return SearchAsync(keyword).GetAwaiter().GetResult();
        }

        public async Task<List<Teacher>> SearchAsync(string keyword)
        {
            var teachers = new List<Teacher>();
            var sql = @"SELECT Id, Name, Phone, Address, ReferenceId, UserId, CreatedDate, ModifiedDate 
                        FROM Teachers 
                        WHERE Name LIKE @Keyword OR Phone LIKE @Keyword OR Address LIKE @Keyword 
                        ORDER BY Name";

            var parameters = new Dictionary<string, object> { { "@Keyword", $"%{keyword}%" } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    teachers.Add(new Teacher
                    {
                        Id = ParseGuid(reader["Id"]),
                        Name = ParseString(reader["Name"]),
                        Phone = ParseString(reader["Phone"]),
                        Address = ParseString(reader["Address"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        UserId = ParseGuid(reader["UserId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    });
                }
            }

            return teachers;
        }
    }
}
