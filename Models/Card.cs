using System;

namespace Memoryy.Models
{
    public class Card
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Card(string imagePath, int row, int column)
        {
            ImagePath = imagePath;
            Row = row;
            Column = column;
            IsFlipped = false;
            IsMatched = false;
        }
    }
}