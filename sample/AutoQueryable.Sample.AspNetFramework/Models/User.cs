using System;

namespace AutoQueryable.Sample.AspNetFramework.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public Address Address { get; set; } = new Address();
    }
}