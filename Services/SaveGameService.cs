using System;
using System.IO;
using System.Text.Json;
using Memoryy.Models;

namespace Memoryy.Services
{
    public class SaveGameService
    {
        private readonly string _saveGamesPath;

        public SaveGameService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Memoryy"
            );
            Directory.CreateDirectory(appDataPath);
            _saveGamesPath = Path.Combine(appDataPath, "SavedGames");
            Directory.CreateDirectory(_saveGamesPath);
        }

        public void SaveGame(SavedGame game)
        {
            string filePath = GetSaveGamePath(game.Username);
            string json = JsonSerializer.Serialize(game);
            File.WriteAllText(filePath, json);
        }

        public SavedGame LoadGame(string username)
        {
            string filePath = GetSaveGamePath(username);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<SavedGame>(json);
            }
            return null;
        }

        public bool HasSavedGame(string username)
        {
            return File.Exists(GetSaveGamePath(username));
        }

        public void DeleteSavedGame(string username)
        {
            string filePath = GetSaveGamePath(username);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetSaveGamePath(string username)
        {
            return Path.Combine(_saveGamesPath, $"{username}.json");
        }
    }
}