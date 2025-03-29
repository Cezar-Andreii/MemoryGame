using System;

namespace Memoryy.Models
{
    public class CustomGameConfiguration
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int TimeLimit { get; set; }

        public bool IsValid()
        {
            return Rows >= 2 && Rows <= 6 &&
                   Columns >= 2 && Columns <= 6 &&
                   (Rows * Columns) % 2 == 0 &&
                   TimeLimit > 0;
        }
    }
}