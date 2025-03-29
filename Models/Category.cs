using System.Collections.Generic;

namespace Memoryy.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<string> ImagePaths { get; set; }

        public Category()
        {
            ImagePaths = new List<string>();
        }
    }
}