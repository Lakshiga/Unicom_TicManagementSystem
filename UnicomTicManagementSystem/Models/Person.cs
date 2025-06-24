using System;

namespace UnicomTicManagementSystem.Models
{
    public abstract class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        protected Person()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        protected Person(string name, string address)
        {
            Name = name;
            Address = address;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public abstract string GetPersonType();
    }
}

