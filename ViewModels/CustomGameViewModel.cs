using System;
using System.ComponentModel;
using System.Windows.Input;
using Memoryy.Models;

namespace Memoryy.ViewModels
{
    public class CustomGameViewModel : INotifyPropertyChanged
    {
        private int _rows = 4;
        private int _columns = 4;
        private int _timeLimit = 300;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Rows
        {
            get => _rows;
            set
            {
                if (value >= 2 && value <= 6)
                {
                    _rows = value;
                    OnPropertyChanged(nameof(Rows));
                }
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                if (value >= 2 && value <= 6)
                {
                    _columns = value;
                    OnPropertyChanged(nameof(Columns));
                }
            }
        }

        public int TimeLimit
        {
            get => _timeLimit;
            set
            {
                if (value > 0)
                {
                    _timeLimit = value;
                    OnPropertyChanged(nameof(TimeLimit));
                }
            }
        }

        public ICommand StartCommand => new RelayCommand(StartGame, CanStartGame);
        public ICommand CancelCommand => new RelayCommand(Cancel);

        private bool CanStartGame()
        {
            return (Rows * Columns) % 2 == 0;
        }

        private void StartGame()
        {
            var config = new GameConfiguration
            {
                Rows = Rows,
                Columns = Columns,
                TimeLimit = TimeLimit,
                StartTime = DateTime.Now
            };

            if (config.IsValid())
            {
                // Notificăm fereastra principală despre configurația aleasă
                OnGameConfigurationSelected?.Invoke(this, config);
            }
        }

        private void Cancel()
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<GameConfiguration> OnGameConfigurationSelected;
        public event EventHandler OnCancel;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}