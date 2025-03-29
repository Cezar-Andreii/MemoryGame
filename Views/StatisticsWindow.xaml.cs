using System.Windows;
using Memoryy.ViewModels;

namespace Memoryy.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel();
        }
    }
}