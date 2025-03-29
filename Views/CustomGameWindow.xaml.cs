using System.Windows;
using System.Windows.Controls;
using Memoryy.ViewModels;

namespace Memoryy.Views
{
    public partial class CustomGameWindow : Window
    {
        private readonly CustomGameViewModel _viewModel;

        public CustomGameWindow(CustomGameViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var dimensions = button.Content.ToString().Split('x');
                if (dimensions.Length == 2 &&
                    int.TryParse(dimensions[0], out int rows) &&
                    int.TryParse(dimensions[1], out int columns))
                {
                    _viewModel.Rows = rows;
                    _viewModel.Columns = columns;
                }
            }
        }
    }
}