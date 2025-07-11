using System;
using System.ComponentModel.DataAnnotations;

namespace QuickMind.Models
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

        // SM-2 параметры
        public int Interval { get; set; } = 1; // Интервал в днях до следующего показа
        public int Repetitions { get; set; } = 0; // Количество успешных повторений
        public double EaseFactor { get; set; } = 2.5; // Коэффициент легкости (SM-2)
        public DateTime DueDate { get; set; } = DateTime.Today; // Дата следующего показа
        
        // Новые поля для улучшенного алгоритма
        public CardType Type { get; set; } = CardType.New; // Тип карточки (новые/обучаемые/повторяемые)
        public int LearningStep { get; set; } = 0; // Текущий шаг обучения (для многоступенчатого обучения)
        public int Lapses { get; set; } = 0; // Количество ошибок (для определения leech-карточек)
        public bool IsLeech { get; set; } = false; // Помечена ли как leech
        public bool IsSuspended { get; set; } = false; // Приостановлена ли карточка
        public int FuzzFactor { get; set; } = 0; // Случайный сдвиг интервала (fuzzing)
        
        // Настройки для разных состояний
        public DateTime? LearningDueDate { get; set; } // Дата следующего показа в режиме обучения
        public int GraduatingInterval { get; set; } = 1; // Интервал после окончания обучения
        public int EasyInterval { get; set; } = 4; // Интервал для "легких" карточек
    }
    
    public enum CardStatus
    {
        New,        // Новые карточки
        Learning,   // Обучаемые карточки
        Review,     // Карточки на повторении
        Relearning  // Переобучаемые карточки
    }
    
    public enum CardType
    {
        New,        // Новые карточки (никогда не показывались)
        Learning,   // Обучаемые карточки (проходят через шаги обучения)
        Review,     // Карточки на повторении (основная очередь)
        Relearning  // Переобучаемые карточки (после ошибки)
    }
} 