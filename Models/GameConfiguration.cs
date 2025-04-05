using System;

namespace Memoryy.Models
{
    public class GameConfiguration
    {
        public int Rows { get; set; } = 4;
        public int Columns { get; set; } = 4;
        public int TimeLimit { get; set; } = 300; // 5 minute în secunde
        public string CategoryName { get; set; } = "Default";
        public DateTime StartTime { get; set; }
        public DateTime? LastSaveTime { get; set; }

        public bool IsValid()
        {
            return Rows >= 2 && Rows <= 6 &&
                   Columns >= 2 && Columns <= 6 &&
                   (Rows * Columns) % 2 == 0 &&
                   TimeLimit > 0;
        }
    }
}