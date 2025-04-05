using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoryy.Models;

namespace Memoryy.Services
{
    public class CategoryService
    {
        private readonly string _imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        private List<Category> _categories;

        public CategoryService()
        {
            try
            {
                InitializeCategories();

                _categories = new List<Category>
                {
                    new Category { Name = "Animale", ImagePaths = GetImagePaths("Animale") },
                    new Category { Name = "Fructe", ImagePaths = GetImagePaths("Fructe") },
                    new Category { Name = "Emoji", ImagePaths = GetImagePaths("Emoji") }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la inițializarea categoriilor: {ex.Message}");
            }
        }

        private void InitializeCategories()
        {
            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }

            var categoryNames = new[] { "Animale", "Fructe", "Emoji" };
            foreach (var categoryName in categoryNames)
            {
                var categoryPath = Path.Combine(_imagesPath, categoryName);
                if (!Directory.Exists(categoryPath))
                {
                    Directory.CreateDirectory(categoryPath);
                }
            }
        }

        private List<string> GetImagePaths(string categoryFolder)
        {
            var paths = new List<string>();
            string categoryPath = Path.Combine(_imagesPath, categoryFolder);

            if (Directory.Exists(categoryPath))
            {
                paths.AddRange(Directory.GetFiles(categoryPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                 file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                 file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)));
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

        public List<string> GetImagesForCategory(string category)
        {
            var categoryPath = Path.Combine(_imagesPath, category);
            if (!Directory.Exists(categoryPath))
            {
                return new List<string>();
            }

            return Directory.GetFiles(categoryPath, "*.png")
                          .Concat(Directory.GetFiles(categoryPath, "*.jpg"))
                          .Concat(Directory.GetFiles(categoryPath, "*.jpeg"))
                          .ToList();
        }
    }
}