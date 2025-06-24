using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public class RoomRepository : BaseRepository<Room>
    {
        public override List<Room> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public override async Task<List<Room>> GetAllAsync()
        {
            var rooms = new List<Room>();
            var sql = "SELECT Id, RoomName, RoomType, ReferenceId, CreatedDate, ModifiedDate FROM Rooms ORDER BY RoomName";

            using (var reader = await ExecuteReaderAsync(sql))
            {
                while (await reader.ReadAsync())
                {
                    rooms.Add(new Room
                    {
                        Id = ParseGuid(reader["Id"]),
                        RoomName = ParseString(reader["RoomName"]),
                        RoomType = ParseString(reader["RoomType"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    });
                }
            }

            return rooms;
        }

        public override Room GetById(Guid id)
        {
            // Keep synchronous version for backward compatibility
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public override async Task<Room> GetByIdAsync(Guid id)
        {
            var sql = "SELECT Id, RoomName, RoomType, ReferenceId, CreatedDate, ModifiedDate FROM Rooms WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    return new Room
                    {
                        Id = ParseGuid(reader["Id"]),
                        RoomName = ParseString(reader["RoomName"]),
                        RoomType = ParseString(reader["RoomType"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    };
                }
            }

            return null;
        }

        public override void Add(Room entity)
        {
            // Keep synchronous version for backward compatibility
            AddAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task AddAsync(Room entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"INSERT INTO Rooms (Id, RoomName, RoomType, ReferenceId, CreatedDate, ModifiedDate) 
                       VALUES (@Id, @RoomName, @RoomType, @ReferenceId, @CreatedDate, @ModifiedDate)";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@RoomName", entity.RoomName },
                { "@RoomType", entity.RoomType },
                { "@ReferenceId", entity.ReferenceId },
                { "@CreatedDate", entity.CreatedDate },
                { "@ModifiedDate", entity.ModifiedDate }
            };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public override void Update(Room entity)
        {
            // Keep synchronous version for backward compatibility
            UpdateAsync(entity).GetAwaiter().GetResult();
        }

        public override async Task UpdateAsync(Room entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.ModifiedDate = DateTime.UtcNow;

            var sql = @"UPDATE Rooms 
                       SET RoomName = @RoomName, RoomType = @RoomType, 
                           ReferenceId = @ReferenceId, ModifiedDate = @ModifiedDate 
                       WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id.ToString() },
                { "@RoomName", entity.RoomName },
                { "@RoomType", entity.RoomType },
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
            var sql = "DELETE FROM Rooms WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id.ToString() } };

            await ExecuteNonQueryAsync(sql, parameters);
        }

        public List<Room> GetRoomsByType(string roomType)
        {
            // Keep synchronous version for backward compatibility
            return GetRoomsByTypeAsync(roomType).GetAwaiter().GetResult();
        }

        public async Task<List<Room>> GetRoomsByTypeAsync(string roomType)
        {
            var rooms = new List<Room>();
            var sql = "SELECT Id, RoomName, RoomType, ReferenceId, CreatedDate, ModifiedDate FROM Rooms WHERE RoomType = @RoomType ORDER BY RoomName";
            var parameters = new Dictionary<string, object> { { "@RoomType", roomType } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                while (await reader.ReadAsync())
                {
                    rooms.Add(new Room
                    {
                        Id = ParseGuid(reader["Id"]),
                        RoomName = ParseString(reader["RoomName"]),
                        RoomType = ParseString(reader["RoomType"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    });
                }
            }

            return rooms;
        }

        public Room GetByName(string roomName)
        {
            // Keep synchronous version for backward compatibility
            return GetByNameAsync(roomName).GetAwaiter().GetResult();
        }

        public async Task<Room> GetByNameAsync(string roomName)
        {
            var sql = "SELECT Id, RoomName, RoomType, ReferenceId, CreatedDate, ModifiedDate FROM Rooms WHERE RoomName = @RoomName";
            var parameters = new Dictionary<string, object> { { "@RoomName", roomName } };

            using (var reader = await ExecuteReaderAsync(sql, parameters))
            {
                if (await reader.ReadAsync())
                {
                    return new Room
                    {
                        Id = ParseGuid(reader["Id"]),
                        RoomName = ParseString(reader["RoomName"]),
                        RoomType = ParseString(reader["RoomType"]),
                        ReferenceId = ParseInt(reader["ReferenceId"]),
                        CreatedDate = ParseDateTime(reader["CreatedDate"]),
                        ModifiedDate = ParseDateTime(reader["ModifiedDate"])
                    };
                }
            }

            return null;
        }
    }
}