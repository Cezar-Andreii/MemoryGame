using System;
using System.Collections.Generic;
using System.IO;
using Memoryy.Models;

namespace Memoryy.Services
{
    public class CategoryService
    {
        private readonly List<Category> _categories;

        public CategoryService()
        {
            _categories = new List<Category>
            {
                new Category { Name = "Animale", ImagePaths = GetImagePaths("Animals") },
                new Category { Name = "Fructe", ImagePaths = GetImagePaths("Fruits") },
                new Category { Name = "Emoji", ImagePaths = GetImagePaths("Emojis") }
            };
        }

        private List<string> GetImagePaths(string categoryFolder)
        {
            var paths = new List<string>();
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", categoryFolder);

            if (Directory.Exists(basePath))
            {
                paths.AddRange(Directory.GetFiles(basePath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                 file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                 file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)));
            }

            return paths;
        }

        public List<Category> GetAllCategories()
        {
            return _categories;
        }

        public Category GetCategoryByName(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == name);
        }
    }
}