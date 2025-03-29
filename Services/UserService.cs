using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Memoryy.Models;

namespace Memoryy.Services
{
    public class UserService
    {
        private readonly string _usersFilePath;
        private readonly SaveGameService _saveGameService;
        private readonly StatisticsService _statisticsService;
        private List<User> _users;

        public UserService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Memoryy"
            );
            Directory.CreateDirectory(appDataPath);
            _usersFilePath = Path.Combine(appDataPath, "users.json");
            _saveGameService = new SaveGameService();
            _statisticsService = new StatisticsService();
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(_usersFilePath))
            {
                string json = File.ReadAllText(_usersFilePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            else
            {
                _users = new List<User>();
            }
        }

        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(_users);
            File.WriteAllText(_usersFilePath, json);
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public void AddUser(User user)
        {
            _users.Add(user);
            SaveUsers();
        }

        public void DeleteUser(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                // Ștergem jocul salvat al utilizatorului
                _saveGameService.DeleteSavedGame(username);

                // Ștergem statisticile utilizatorului
                _statisticsService.DeleteUserStatistics(username);

                // Ștergem utilizatorul din listă
                _users.Remove(user);
                SaveUsers();
            }
        }

        public bool UserExists(string username)
        {
            return _users.Exists(u => u.Username == username);
        }
    }
}