using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FlashCardApp.Models
{
    public class FlashCardContext : DbContext
    {
        public DbSet<FlashCard> FlashCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlashCardApp");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            var dbPath = Path.Combine(appDataPath, "flashcards.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlashCard>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Question).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Answer).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasConversion<string>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
} 