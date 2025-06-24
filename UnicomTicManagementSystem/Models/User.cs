using System;

namespace UnicomTicManagementSystem.Models
{
    public class User
    {
        private static int _lastReferenceId = 0;


        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
            IsActive = true;
        }

        public static User CreateUser(string username, string password, string role)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = password,
                Role = role,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId,
                IsActive = true
            };
        }
    }
}
