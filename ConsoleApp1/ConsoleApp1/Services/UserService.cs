using ConsoleApp1.Data;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Services
{
    public class UserService
    {
        private readonly ShelterContext _db;
        public UserService(ShelterContext db) => _db = db;

        public User AddUser(string name)
        {
            var u = new User { Name = name };
            _db.Users.Add(u);
            _db.SaveChanges();
            return u;
        }

        public User? GetUser(int id) => _db.Users.Include(u => u.Movies).FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() => _db.Users.Include(u => u.Movies).AsNoTracking().ToList();
    }
}
