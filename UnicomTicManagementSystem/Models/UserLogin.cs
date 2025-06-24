using System;

namespace UnicomTicManagementSystem.Models
{
    public class UserLogin
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string Stream { get; private set; }
        public int ReferenceId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private UserLogin()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static UserLogin CreateUserLogin(string username, string password, string role, string name, string address, string stream)
        {
            return new UserLogin
            {
                Username = username,
                Password = password,
                Role = role,
                Name = name,
                Address = address,
                Stream = stream,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
