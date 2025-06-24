using System;

namespace UnicomTicManagementSystem.Models
{ 
    public class Student : Person
    {
        private static int _lastReferenceId = 0;


        public string Username { get; set; }
        public string Password { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; }
        public string Stream { get; set; }
        public int ReferenceId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? LastAttendanceDate { get; set; }
        public bool IsActive { get; set; }

        public Student() : base()
        {
            Id = Guid.NewGuid();
            ReferenceId = ++_lastReferenceId;
            IsActive = true;
        }

        public Student(Guid id, string studentName, int referenceId)
        {
            Id = id;
            Name = studentName;
            ReferenceId = referenceId;
            IsActive = true;
        }

        public override string GetPersonType()
        {
            return "Student";
        }

        // ✅ Now this will work
        public static Student CreateStudent(Guid id, string studentName, int referenceId)
        {
            return new Student(id, studentName, referenceId);
        }
    }
}