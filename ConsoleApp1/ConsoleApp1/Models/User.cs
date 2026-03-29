using System.Collections.Generic;

namespace ConsoleApp1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Movie> Movies { get; set; } = new();
    }
}
