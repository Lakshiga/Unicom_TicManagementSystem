using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class SectionRepository : BaseRepository<Section>
    {
        private const string TableName = "Sections";

        public override List<Section> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public override async Task<List<Section>> GetAllAsync()
        {
            var sections = new List<Section>();
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                ORDER BY Name";

            using (var reader = await ExecuteReaderAsync(sql))
            {
                while (await reader.ReadAsync())
                {
                    sections.Add(MapReaderToSection(reader));
                }
            }

            return sections;
        }

        public override Section GetById(Guid id)
        {
            // Keep synchronous version for backward compatibility
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public override async Task<Section> GetByIdAsync(Guid id)
        {
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    return MapReaderToSection(reader);
                }
            }

            return null;
        }

        public override void Add(Section entity)
        {
            // Keep synchronous version for backward compatibility
            AddAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task AddAsync(Section entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = $@"
                INSERT INTO {TableName} (Id, Name, ReferenceId, CreatedDate, ModifiedDate) 
                VALUES (@Id, @Name, @ReferenceId, @CreatedDate, @ModifiedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@ReferenceId", entity.ReferenceId },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Update(Section entity)
        {
            // Keep synchronous version for backward compatibility
            UpdateAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task UpdateAsync(Section entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = $@"
                UPDATE {TableName} 
                SET Name = @Name, ReferenceId = @ReferenceId, ModifiedDate = @ModifiedDate 
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@Name", entity.Name },
                { "@ReferenceId", entity.ReferenceId },
                { "@ModifiedDate", DateTime.UtcNow }
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
            var sql = $"DELETE FROM {TableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        /// <summary>
        /// Gets a section by name (case-insensitive)
        /// </summary>
        /// <param name="name">Section name</param>
        /// <returns>Section if found, null otherwise</returns>
        public Section GetByName(string name)
        {
            // Keep synchronous version for backward compatibility
            return GetByNameAsync(name).GetAwaiter().GetResult();
        }

        public async Task<Section> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                WHERE LOWER(Name) = LOWER(@Name)";

            var parameters = new Dictionary<string, object> { { "@Name", name.Trim() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    return MapReaderToSection(reader);
                }
            }

            return null;
        }

        /// <summary>
        /// Searches sections by name pattern
        /// </summary>
        /// <param name="searchPattern">Search pattern (supports % wildcard)</param>
        /// <returns>List of matching sections</returns>
        public List<Section> SearchByName(string searchPattern)
        {
            // Keep synchronous version for backward compatibility
            return SearchByNameAsync(searchPattern).GetAwaiter().GetResult();
        }

        public async Task<List<Section>> SearchByNameAsync(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
                return new List<Section>();

            var sections = new List<Section>();
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                WHERE Name LIKE @SearchPattern 
                ORDER BY Name";

            var parameters = new Dictionary<string, object> { { "@SearchPattern", $"%{searchPattern}%" } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    sections.Add(MapReaderToSection(reader));
                }
            }

            return sections;
        }

        /// <summary>
        /// Gets sections created within a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of sections created in the date range</returns>
        public List<Section> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            // Keep synchronous version for backward compatibility
            return GetByDateRangeAsync(startDate, endDate).GetAwaiter().GetResult();
        }

        public async Task<List<Section>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sections = new List<Section>();
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                WHERE CreatedDate BETWEEN @StartDate AND @EndDate 
                ORDER BY CreatedDate DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@StartDate", startDate },
                { "@EndDate", endDate }
            };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    sections.Add(MapReaderToSection(reader));
                }
            }

            return sections;
        }

        /// <summary>
        /// Gets sections with pagination
        /// </summary>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="limit">Number of records to return</param>
        /// <param name="searchTerm">Optional search term</param>
        /// <returns>List of sections for the specified page</returns>
        public List<Section> GetPaged(int offset, int limit, string searchTerm = null)
        {
            // Keep synchronous version for backward compatibility
            return GetPagedAsync(offset, limit, searchTerm).GetAwaiter().GetResult();
        }

        public async Task<List<Section>> GetPagedAsync(int offset, int limit, string searchTerm = null)
        {
            var sections = new List<Section>();
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName}";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += " WHERE Name LIKE @SearchTerm";
                parameters.Add("@SearchTerm", $"%{searchTerm}%");
            }

            sql += " ORDER BY Name LIMIT @Limit OFFSET @Offset";
            parameters.Add("@Limit", limit);
            parameters.Add("@Offset", offset);

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    sections.Add(MapReaderToSection(reader));
                }
            }

            return sections;
        }

        /// <summary>
        /// Gets the total count of sections
        /// </summary>
        /// <param name="searchTerm">Optional search term</param>
        /// <returns>Total count of sections</returns>
        public int GetCount(string searchTerm = null)
        {
            // Keep synchronous version for backward compatibility
            return GetCountAsync(searchTerm).GetAwaiter().GetResult();
        }

        public async Task<int> GetCountAsync(string searchTerm = null)
        {
            var sql = $"SELECT COUNT(*) FROM {TableName}";
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += " WHERE Name LIKE @SearchTerm";
                parameters.Add("@SearchTerm", $"%{searchTerm}%");
            }

            var result = await ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Checks if a section name already exists
        /// </summary>
        /// <param name="name">Section name to check</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if name exists, false otherwise</returns>
        public bool NameExists(string name, Guid? excludeId = null)
        {
            // Keep synchronous version for backward compatibility
            return NameExistsAsync(name, excludeId).GetAwaiter().GetResult();
        }

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var sql = $@"
                SELECT COUNT(*) 
                FROM {TableName} 
                WHERE LOWER(Name) = LOWER(@Name)";

            var parameters = new Dictionary<string, object> { { "@Name", name.Trim() } };

            if (excludeId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("@ExcludeId", excludeId.Value.ToString());
            }

            var result = await ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Gets sections that were recently modified
        /// </summary>
        /// <param name="days">Number of days to look back</param>
        /// <returns>List of recently modified sections</returns>
        public List<Section> GetRecentlyModified(int days = 7)
        {
            // Keep synchronous version for backward compatibility
            return GetRecentlyModifiedAsync(days).GetAwaiter().GetResult();
        }

        public async Task<List<Section>> GetRecentlyModifiedAsync(int days = 7)
        {
            var sections = new List<Section>();
            var sql = $@"
                SELECT Id, Name, ReferenceId, CreatedDate, ModifiedDate 
                FROM {TableName} 
                WHERE ModifiedDate >= @StartDate 
                ORDER BY ModifiedDate DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@StartDate", DateTime.UtcNow.AddDays(-days) }
            };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    sections.Add(MapReaderToSection(reader));
                }
            }

            return sections;
        }

        #region Private Methods

        /// <summary>
        /// Maps a SQLiteDataReader row to a Section object
        /// </summary>
        /// <param name="reader">SQLiteDataReader</param>
        /// <returns>Section object</returns>
        private Section MapReaderToSection(SQLiteDataReader reader)
        {
            var id = ParseGuid(reader["Id"]);
            var name = ParseString(reader["Name"]);
            var createdDate = ParseDateTime(reader["CreatedDate"]);
            var modifiedDate = ParseDateTime(reader["ModifiedDate"]);

            var section = new Section(id, name, createdDate, modifiedDate);
            section.SetReferenceId(ParseInt(reader["ReferenceId"]));

            return section;
        }

        #endregion
    }
}