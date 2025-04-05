using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Memoryy.Models
{
    public class Card : INotifyPropertyChanged
    {
        private string _imagePath;
        private bool _isFlipped;
        private bool _isMatched;
        private int _row;
        private int _column;

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                _isFlipped = value;
                OnPropertyChanged();
            }
        }

        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                _isMatched = value;
                OnPropertyChanged();
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                _row = value;
                OnPropertyChanged();
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Card(string imagePath, int row, int column)
        {
            ImagePath = imagePath;
            Row = row;
            Column = column;
            IsFlipped = false;
            IsMatched = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}