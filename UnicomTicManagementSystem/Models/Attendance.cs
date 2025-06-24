using System;

namespace UnicomTicManagementSystem.Models
{
    public class Attendance
    {

        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid SubjectId { get; private set; }
        public DateTime Date { get; private set; }
        public string Status { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private Attendance()
        {
            Id = Guid.NewGuid(); 
            CreatedDate = DateTime.Now; 
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static Attendance CreateAttendance(Guid studentId, Guid subjectId, DateTime date, string status)
        {
            var attendance = new Attendance();
            attendance.StudentId = studentId; 
            attendance.SubjectId = subjectId; 
            attendance.Date = date; 
            attendance.Status = status; 
            attendance.ModifiedDate = DateTime.Now; 
            return attendance;
        }

        public void SetReferenceId(int referenceId)
        {
            var property = typeof(Attendance).GetProperty("ReferenceId");
            property.SetValue(this, referenceId);
        }
    }
}