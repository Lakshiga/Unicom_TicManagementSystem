using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models; // Needed for TimeTable model

namespace UnicomTicManagementSystem.Controllers
{
    public class TimetableController
    {
        private readonly TimeTableRepository repository = new TimeTableRepository();

        public async Task<DataTable> GetSubjectsAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT SubjectName FROM Subjects";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public async Task<DataTable> GetRoomsAsync()
        {
            using (var conn = DbCon.GetConnection())
            {
                string query = "SELECT RoomName FROM Rooms";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public async Task<DataTable> GetAllTimetablesAsync()
        {
            return await repository.GetAllTimeTablesAsync();
        }

        public async Task AddTimetableAsync(string subject, string timeSlot, string room, DateTime date)
        {
            var timetable = TimeTable.CreateTimeTable(subject, timeSlot, room, date);
            await repository.AddTimeTableAsync(timetable);
        }

        public async Task UpdateTimetableAsync(Guid id, string subject, string timeSlot, string room, DateTime date)
        {
            var updated = TimeTable.CreateTimeTable(subject, timeSlot, room, date);

            // Use existing Id — required for update
            typeof(TimeTable).GetProperty("Id").SetValue(updated, id);

            await repository.UpdateTimeTableAsync(updated);
        }

        public async Task DeleteTimetableAsync(Guid id)
        {
            await repository.DeleteTimeTableAsync(id);
        }
    }
}
