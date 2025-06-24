using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnicomTicManagementSystem.Data
{
    public static class DbCon
    {
        private static  string ConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "unicomtic.db")};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            try
            {
                var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Failed to establish database connection: {ex.Message}. Connection string: {ConnectionString}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while connecting to database: {ex.Message}. Connection string: {ConnectionString}", ex);
            }
        }
        public static string GetConnectionString()
        {
            return ConnectionString;
        }
    }
}
