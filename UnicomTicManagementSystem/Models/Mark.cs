using System;

namespace UnicomTicManagementSystem.Models
{
    public class Mark
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public string Subject { get; private set; }
        public string Exam { get; private set; }
        public int Score { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private Mark()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static Mark CreateMark(Guid studentId, string subject, string exam, int score)
        {
            return new Mark
            {
                StudentId = studentId,
                Subject = subject,
                Exam = exam,
                Score = score,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }
    }
}
