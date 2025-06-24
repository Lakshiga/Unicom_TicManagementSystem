using System;

namespace UnicomTicManagementSystem.Models
{
    public class StudentTeacher
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid TeacherId { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private StudentTeacher()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static StudentTeacher CreateStudentTeacher(Guid studentId, Guid teacherId)
        {
            return new StudentTeacher
            {
                StudentId = studentId,
                TeacherId = teacherId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }
    }
}
