using Microsoft.EntityFrameworkCore;
using EnglishLearningApp.Models;

namespace EnglishLearningApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<WordEntry> Words { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserProgress> UserProgress { get; set; }

        public string DbPath { get; private set; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.ApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "EnglishLearningApp", "app.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var folder = Environment.SpecialFolder.ApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = System.IO.Path.Join(path, "EnglishLearningApp", "app.db");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
            
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Initialize default UserProgress
            modelBuilder.Entity<UserProgress>().HasData(
                new UserProgress { Id = 1 }
            );
        }
    }
}
