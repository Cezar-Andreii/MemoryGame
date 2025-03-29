using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Memoryy.Models;
using Memoryy.Services;
using Memoryy.Views;

namespace Memoryy.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private string _newUsername;
        private string _selectedImagePath;
        private User _selectedUser;
        private bool _isUserSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>(_userService.GetAllUsers());
        }

        public ObservableCollection<User> Users { get; }

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                _newUsername = value;
                OnPropertyChanged(nameof(NewUsername));
            }
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged(nameof(SelectedImagePath));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                IsUserSelected = value != null;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public bool IsUserSelected
        {
            get => _isUserSelected;
            set
            {
                _isUserSelected = value;
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public ICommand SelectImageCommand => new RelayCommand(SelectImage);
        public ICommand CreateUserCommand => new RelayCommand(CreateUser, CanCreateUser);
        public ICommand DeleteUserCommand => new RelayCommand(DeleteUser, () => IsUserSelected);
        public ICommand PlayCommand => new RelayCommand(Play, () => IsUserSelected);

        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedImagePath = dialog.FileName;
            }
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername) &&
                   !string.IsNullOrWhiteSpace(SelectedImagePath) &&
                   !_userService.UserExists(NewUsername);
        }

        private void CreateUser()
        {
            var user = new User
            {
                Username = NewUsername,
                ImagePath = SelectedImagePath
            };

            _userService.AddUser(user);
            Users.Add(user);
            NewUsername = string.Empty;
            SelectedImagePath = string.Empty;
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Sigur doriți să ștergeți utilizatorul {SelectedUser.Username}? Această acțiune va șterge și toate datele asociate acestuia.",
                "Confirmare ștergere",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _userService.DeleteUser(SelectedUser.Username);
                Users.Remove(SelectedUser);
                SelectedUser = null;
                MessageBox.Show("Utilizator șters cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Play()
        {
            var mainWindow = new MainGameWindow(SelectedUser);
            mainWindow.Show();
            Application.Current.MainWindow.Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}