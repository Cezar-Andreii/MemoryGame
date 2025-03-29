using System.Windows;
using Memoryy.ViewModels;

namespace Memoryy.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}