using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Memoryy.Models;

namespace Memoryy.Services
{
    public class StatisticsService
    {
        private readonly string _statisticsFilePath;
        private Dictionary<string, UserStatistics> _statistics;

        public StatisticsService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Memoryy"
            );
            Directory.CreateDirectory(appDataPath);
            _statisticsFilePath = Path.Combine(appDataPath, "statistics.json");
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            if (File.Exists(_statisticsFilePath))
            {
                string json = File.ReadAllText(_statisticsFilePath);
                _statistics = JsonSerializer.Deserialize<Dictionary<string, UserStatistics>>(json)
                            ?? new Dictionary<string, UserStatistics>();
            }
            else
            {
                _statistics = new Dictionary<string, UserStatistics>();
            }
        }

        private void SaveStatistics()
        {
            string json = JsonSerializer.Serialize(_statistics);
            File.WriteAllText(_statisticsFilePath, json);
        }

        public void AddGamePlayed(string username)
        {
            if (!_statistics.ContainsKey(username))
            {
                _statistics[username] = new UserStatistics { Username = username };
            }

            _statistics[username].GamesPlayed++;
            _statistics[username].LastUpdated = DateTime.Now;
            SaveStatistics();
        }

        public void AddGameWon(string username)
        {
            if (!_statistics.ContainsKey(username))
            {
                _statistics[username] = new UserStatistics { Username = username };
            }

            _statistics[username].GamesWon++;
            _statistics[username].LastUpdated = DateTime.Now;
            SaveStatistics();
        }

        public List<UserStatistics> GetAllStatistics()
        {
            return _statistics.Values.OrderByDescending(s => s.GamesWon).ToList();
        }

        public void DeleteUserStatistics(string username)
        {
            if (_statistics.ContainsKey(username))
            {
                _statistics.Remove(username);
                SaveStatistics();
            }
        }
    }
}