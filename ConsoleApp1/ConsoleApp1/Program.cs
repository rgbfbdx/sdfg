using System;
using System.Linq;
using ConsoleApp1.Data;
using ConsoleApp1.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            using var db = new ShelterContext();
            db.Database.EnsureCreated();

            // Create view if not exists
            var createView = @"CREATE VIEW IF NOT EXISTS UserMovies AS
SELECT Users.Id as UserId, Users.Name as UserName, Movies.Id as MovieId, Movies.Title as MovieTitle, Movies.Year as MovieYear
FROM Users LEFT JOIN Movies ON Users.Id = Movies.UserId;";
            db.Database.ExecuteSqlRaw(createView);

            Console.WriteLine("Shelter app - Users and Movies");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add user (via 'procedure')");
                Console.WriteLine("2. View users and their movies (view)");
                Console.WriteLine("3. Add movie to user");
                Console.WriteLine("4. View all users (EF Core)");
                Console.WriteLine("5. Exit");
                Console.Write("Select: ");
                var s = Console.ReadLine();
                if (s == "5") break;

                switch (s)
                {
                    case "1":
                        Console.Write("User name: ");
                        var name = Console.ReadLine() ?? string.Empty;
                        var newId = AddUserProcedure(name);
                        Console.WriteLine($"Added user with Id={newId}");
                        break;
                    case "2":
                        using var conn = new SqliteConnection("Data Source=shelter.db");
                        conn.Open();
                        var rows = conn.Query<UserMovieRow>("SELECT * FROM UserMovies").ToList();
                        foreach (var r in rows)
                        {
                            Console.WriteLine($"User: {r.UserId} {r.UserName} => Movie: {r.MovieId} {r.MovieTitle} ({r.MovieYear})");
                        }
                        break;
                    case "3":
                        Console.Write("UserId: ");
                        if (!int.TryParse(Console.ReadLine(), out var uid)) break;
                        using var db2 = new ShelterContext();
                        var us = db2.Users.Find(uid);
                        if (us == null) { Console.WriteLine("User not found"); break; }
                        Console.Write("Title: ");
                        var title = Console.ReadLine() ?? string.Empty;
                        Console.Write("Year: ");
                        var year = int.TryParse(Console.ReadLine(), out var y) ? y : 0;
                        var m = new Movie { Title = title, Year = year, UserId = uid };
                        db2.Movies.Add(m);
                        db2.SaveChanges();
                        Console.WriteLine($"Added movie with Id={m.Id}");
                        break;
                    case "4":
                        using var db3 = new ShelterContext();
                        var users = db3.Users.Include(u => u.Movies).ToList();
                        foreach (var u in users)
                        {
                            Console.WriteLine($"User {u.Id}: {u.Name}");
                            foreach (var mv in u.Movies)
                                Console.WriteLine($"  - Movie {mv.Id}: {mv.Title} ({mv.Year})");
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown");
                        break;
                }
            }
        }

        // SQLite does not support stored procedures; emulate by an atomic parameterized insert via Dapper
        static int AddUserProcedure(string name)
        {
            using var conn = new SqliteConnection("Data Source=shelter.db");
            conn.Open();
            // Using a transaction to emulate procedure atomicity
            using var tran = conn.BeginTransaction();
            try
            {
                var sql = "INSERT INTO Users (Name) VALUES (@Name); SELECT last_insert_rowid();";
                var id = conn.ExecuteScalar<long>(sql, new { Name = name }, tran);
                tran.Commit();
                return (int)id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // Helper mapping for view rows
        class UserMovieRow
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public int? MovieId { get; set; }
            public string? MovieTitle { get; set; }
            public int? MovieYear { get; set; }
        }
    }
}
