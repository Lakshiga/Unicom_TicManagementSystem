using System;

namespace UnicomTicManagementSystem.Models
{
    public class Section
    {
        private static int _lastReferenceId = 0; 

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private Section(Guid id, string name)
        {
            Id = id;
            Name = name;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
            ReferenceId = ++_lastReferenceId; 
        }

        public Section(Guid id, string name, DateTime createdDate, DateTime modifiedDate)
        {
            Id = id;
            Name = name;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }

        public static Section CreateSection(string name)
        {
            return new Section(Guid.NewGuid(), name);
        }

        public void SetReferenceId(int referenceId)
        {
            ReferenceId = referenceId;
            if (referenceId > _lastReferenceId)
            {
                _lastReferenceId = referenceId; 
            }
        }
        public void GetReferenceId(int referenceId)
        {
            ReferenceId = referenceId;
        }
        public void UpdateModifiedDate()
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}