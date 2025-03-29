using System.Windows;
using Memoryy.Models;
using Memoryy.ViewModels;

namespace Memoryy.Views
{
    public partial class MainGameWindow : Window
    {
        public MainGameWindow(User currentUser)
        {
            InitializeComponent();
            DataContext = new MainGameViewModel(currentUser);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}