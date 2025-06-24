using System;

namespace UnicomTicManagementSystem.Models
{
    public class Staff : Person
    {
        private static int _lastReferenceId = 0;

        public string Email { get; set; }
        public int ReferenceId { get; set; }
        public Guid UserId { get; set; }

        public Staff() : base()
        {
            ReferenceId = ++_lastReferenceId;
        }

        public Staff(string name, string address, string email, Guid userId) : base(name, address)
        {
            Email = email;
            UserId = userId;
            ReferenceId = ++_lastReferenceId;
        }

        public static Staff CreateStaff(string name, string address, string email, Guid userId)
        {
            return new Staff(name, address, email, userId);
        }

        public override string GetPersonType()
        {
            return "Staff";
        }
    }
}
