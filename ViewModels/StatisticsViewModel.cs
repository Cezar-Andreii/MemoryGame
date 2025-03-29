using System.Collections.ObjectModel;
using System.ComponentModel;
using Memoryy.Models;
using Memoryy.Services;

namespace Memoryy.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly StatisticsService _statisticsService;
        private ObservableCollection<UserStatistics> _statistics;

        public event PropertyChangedEventHandler PropertyChanged;

        public StatisticsViewModel()
        {
            _statisticsService = new StatisticsService();
            Statistics = new ObservableCollection<UserStatistics>(_statisticsService.GetAllStatistics());
        }

        public ObservableCollection<UserStatistics> Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged(nameof(Statistics));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}