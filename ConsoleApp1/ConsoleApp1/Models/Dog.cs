using System;

namespace ConsoleApp1.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public bool IsAdopted { get; set; } = false;
    }
}
