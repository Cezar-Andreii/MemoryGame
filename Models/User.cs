using System;

namespace Memoryy.Models
{
    public class User
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}