namespace ConsoleApp1.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
