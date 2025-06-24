using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Data
{
    public class TimeTableRepository
    {
        public async Task<DataTable> GetAllTimeTablesAsync()
        {
            DataTable table = new DataTable();
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT * FROM Timetable";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public async Task AddTimeTableAsync(TimeTable timetable)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
                    INSERT INTO Timetable 
                    (Id, Subject, TimeSlot, Room, Date, ReferenceId, CreatedDate, ModifiedDate) 
                    VALUES 
                    (@Id, @Subject, @TimeSlot, @Room, @Date, @ReferenceId, @CreatedDate, @ModifiedDate)";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", timetable.Id.ToString());
                    cmd.Parameters.AddWithValue("@Subject", timetable.Subject);
                    cmd.Parameters.AddWithValue("@TimeSlot", timetable.TimeSlot);
                    cmd.Parameters.AddWithValue("@Room", timetable.Room);
                    cmd.Parameters.AddWithValue("@Date", timetable.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@ReferenceId", timetable.ReferenceId);
                    cmd.Parameters.AddWithValue("@CreatedDate", timetable.CreatedDate);
                    cmd.Parameters.AddWithValue("@ModifiedDate", timetable.ModifiedDate);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateTimeTableAsync(TimeTable timetable)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = @"
                    UPDATE Timetable 
                    SET 
                        Subject = @Subject, 
                        TimeSlot = @TimeSlot, 
                        Room = @Room, 
                        Date = @Date, 
                        ModifiedDate = @ModifiedDate 
                    WHERE Id = @Id";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", timetable.Id.ToString());
                    cmd.Parameters.AddWithValue("@Subject", timetable.Subject);
                    cmd.Parameters.AddWithValue("@TimeSlot", timetable.TimeSlot);
                    cmd.Parameters.AddWithValue("@Room", timetable.Room);
                    cmd.Parameters.AddWithValue("@Date", timetable.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now); // only updating modified date

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteTimeTableAsync(Guid id)
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "DELETE FROM Timetable WHERE Id = @Id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id.ToString());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
