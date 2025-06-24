using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Models
{
    public class TeacherSection
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public Guid TeacherId { get; private set; }
        public Guid SectionId { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private TeacherSection()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static TeacherSection CreateTeacherSection(Guid teacherId, Guid sectionId)
        {
            if (teacherId == Guid.Empty || sectionId == Guid.Empty)
            {
                throw new ArgumentException("TeacherId and SectionId must be valid GUIDs.");
            }

            return new TeacherSection
            {
                TeacherId = teacherId,
                SectionId = sectionId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }

        public void SetReferenceId(int referenceId)
        {
            var property = typeof(TeacherSection).GetProperty("ReferenceId");
            property.SetValue(this, referenceId);
        }
    }
}
