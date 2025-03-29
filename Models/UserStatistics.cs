using System;

namespace Memoryy.Models
{
    public class UserStatistics
    {
        public string Username { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public DateTime LastUpdated { get; set; }

        public UserStatistics()
        {
            GamesPlayed = 0;
            GamesWon = 0;
            LastUpdated = DateTime.Now;
        }
    }
}