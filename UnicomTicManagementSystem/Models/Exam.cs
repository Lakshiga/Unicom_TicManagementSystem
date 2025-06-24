using System;

namespace UnicomTicManagementSystem.Models
{
    public class Exam
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public string ExamName { get; private set; }
        public Guid SubjectId { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        public void UpdateId(Guid newId)
        {
            Id = newId;
        }

        public static Exam CreateExam(string examName, Guid subjectId)
        {
            return new Exam
            {
                ExamName = examName,
                SubjectId = subjectId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }

    }
}
