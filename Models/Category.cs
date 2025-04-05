using System.Collections.Generic;

namespace Memoryy.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();

        public override string ToString()
        {
            return Name;
        }
    }
}