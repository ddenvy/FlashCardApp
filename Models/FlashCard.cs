using System;
using System.ComponentModel.DataAnnotations;

namespace FlashCardApp.Models
{
    public class FlashCard
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        public string Answer { get; set; } = string.Empty;
        
        [Required]
        public string Topic { get; set; } = string.Empty;
        
        public CardStatus Status { get; set; } = CardStatus.New;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? LastViewedAt { get; set; }
        
        public int ViewCount { get; set; } = 0;
    }
    
    public enum CardStatus
    {
        New,
        Learning,
        Known
    }
} 