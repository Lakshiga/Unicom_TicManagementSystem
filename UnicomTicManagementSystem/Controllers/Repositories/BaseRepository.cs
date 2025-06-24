using System;
using System.Collections.Generic;
using System.Data.SQLite;
using UnicomTicManagementSystem.Data;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Controllers.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly string _connectionString;

        protected BaseRepository()
        {
            _connectionString = DbCon.GetConnectionString();
        }

        protected SQLiteConnection GetConnection()
        {
            try
            {
                var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Failed to establish database connection: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while connecting to database: {ex.Message}", ex);
            }
        }

        protected void ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        Console.WriteLine("SQL: " + sql); // For debugging

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                var key = param.Key.StartsWith("@") ? param.Key : "@" + param.Key;
                                command.Parameters.AddWithValue(key, param.Value ?? DBNull.Value);

                                Console.WriteLine($"Param: {key} = {param.Value}"); // Log param
                            }
                        }

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database operation failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during database operation: {ex.Message}", ex);
            }
        }


        protected SQLiteDataReader ExecuteReader(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                var connection = GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = sql;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key.StartsWith("@") ? param.Key : "@" + param.Key, param.Value);
                    }
                }

                return command.ExecuteReader();
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database query failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during database query: {ex.Message}", ex);
            }
        }

        protected object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key.StartsWith("@") ? param.Key : "@" + param.Key, param.Value);
                            }
                        }

                        return command.ExecuteScalar();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database scalar operation failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during database scalar operation: {ex.Message}", ex);
            }
        }

        protected Guid ParseGuid(object value)
        {
            if (value == null || value == DBNull.Value)
                return Guid.Empty;

            if (value is Guid guid)
                return guid;

            if (Guid.TryParse(value.ToString(), out Guid result))
                return result;

            return Guid.Empty;
        }

        protected DateTime ParseDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return DateTime.MinValue;

            if (value is DateTime dateTime)
                return dateTime;

            if (DateTime.TryParse(value.ToString(), out DateTime result))
                return result;

            return DateTime.MinValue;
        }

        protected int ParseInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is int intValue)
                return intValue;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return 0;
        }

        protected string ParseString(object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public abstract List<T> GetAll();
        public abstract T GetById(Guid id);
        public abstract void Add(T entity);
        public abstract void Update(T entity);
        public abstract void Delete(Guid id);

        public virtual Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        protected async Task<SQLiteDataReader> ExecuteReaderAsync(string sql, Dictionary<string, object> parameters = null)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key.StartsWith("@") ? param.Key : "@" + param.Key, param.Value);
                }
            }
            return (SQLiteDataReader)await command.ExecuteReaderAsync();
        }

        protected async Task ExecuteNonQueryAsync(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key.StartsWith("@") ? param.Key : "@" + param.Key, param.Value);
                        }
                    }
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        protected async Task<object> ExecuteScalarAsync(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key.StartsWith("@") ? param.Key : "@" + param.Key, param.Value);
                        }
                    }
                    return await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
