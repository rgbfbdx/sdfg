using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;

namespace ConsoleApp1.Data
{
    public class ShelterContext : DbContext
    {
        public ShelterContext()
        {
        }

        public ShelterContext(DbContextOptions<ShelterContext> options)
            : base(options)
        {
        }

        public DbSet<Dog> Dogs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // using a local file-based SQLite database
                optionsBuilder.UseSqlite("Data Source=shelter.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(eb =>
            {
                eb.HasKey(u => u.Id);
                eb.Property(u => u.Name).IsRequired();
            });

            modelBuilder.Entity<Movie>(eb =>
            {
                eb.HasKey(m => m.Id);
                eb.Property(m => m.Title).IsRequired();
                eb.HasOne(m => m.User)
                    .WithMany(u => u.Movies)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
