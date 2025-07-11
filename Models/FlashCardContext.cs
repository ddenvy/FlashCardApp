using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace QuickMind.Models
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
                entity.Property(e => e.Type).HasConversion<string>();
                
                // Настройка новых полей для spaced repetition
                entity.Property(e => e.Interval).HasDefaultValue(1);
                entity.Property(e => e.Repetitions).HasDefaultValue(0);
                entity.Property(e => e.EaseFactor).HasDefaultValue(2.5);
                entity.Property(e => e.LearningStep).HasDefaultValue(0);
                entity.Property(e => e.Lapses).HasDefaultValue(0);
                entity.Property(e => e.IsLeech).HasDefaultValue(false);
                entity.Property(e => e.IsSuspended).HasDefaultValue(false);
                entity.Property(e => e.FuzzFactor).HasDefaultValue(1.0);
                entity.Property(e => e.GraduatingInterval).HasDefaultValue(1);
                entity.Property(e => e.EasyInterval).HasDefaultValue(4);
                
                // Индексы для оптимизации запросов
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Topic);
                entity.HasIndex(e => new { e.Status, e.DueDate });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
} 