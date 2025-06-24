using System;

namespace UnicomTicManagementSystem.Models
{
    public class TimeTable
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public string Subject { get; private set; }
        public string TimeSlot { get; private set; }
        public string Room { get; private set; }
        public DateTime Date { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private TimeTable()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static TimeTable CreateTimeTable(string subject, string timeSlot, string room, DateTime date)
        {
            return new TimeTable
            {
                Subject = subject,
                TimeSlot = timeSlot,
                Room = room,
                Date = date,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }
    }
}
