using System;

namespace UnicomTicManagementSystem.Models
{
    public class Subject
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; set; }
        public string SubjectName { get; set; }
        public Guid SectionId { get; set; }
        public int ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        private Subject()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static Subject CreateSubject(string subjectName, Guid sectionId)
        {
            return new Subject
            {
                SubjectName = subjectName,
                SectionId = sectionId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }
    }
}
