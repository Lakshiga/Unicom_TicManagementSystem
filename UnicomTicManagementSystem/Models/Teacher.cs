using System;

namespace UnicomTicManagementSystem.Models
{
    public class Teacher : Person
    {
        private static int _lastReferenceId = 0;

        public string Phone { get; set; }
        public int ReferenceId { get; set; }
        public Guid UserId { get; set; }  // ✅ Associated User ID

        public Teacher() : base()
        {
            ReferenceId = ++_lastReferenceId;
        }

        public Teacher(string name, string address, string phone, Guid userId) : base(name, address)
        {
            Phone = phone;
            ReferenceId = ++_lastReferenceId;
            UserId = userId;
        }

        public static Teacher CreateTeacher(string name, string address, string phone, Guid userId)
        {
            return new Teacher(name, address, phone, userId);
        }

        public override string GetPersonType()
        {
            return "Teacher";
        }
    }
}
