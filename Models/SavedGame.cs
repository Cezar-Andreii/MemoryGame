using System;
using System.Collections.Generic;

namespace Memoryy.Models
{
    public class SavedGame
    {
        public string Username { get; set; }
        public GameConfiguration Configuration { get; set; }
        public List<CardState> Cards { get; set; }
        public DateTime SaveTime { get; set; }

        public SavedGame()
        {
            Cards = new List<CardState>();
        }
    }

    public class CardState
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}