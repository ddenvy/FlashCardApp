using QuickMind.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickMind.Services
{
    public class CardService
    {
        public event Action? CardsChanged;

        public async Task InitializeDatabaseAsync()
        {
            using var context = new FlashCardContext();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task<List<FlashCard>> GetAllCardsAsync()
        {
            using var context = new FlashCardContext();
            return await context.FlashCards.OrderBy(c => c.CreatedAt).ToListAsync();
        }

        public async Task<List<FlashCard>> GetCardsByTopicAsync(string topic)
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Where(c => c.Topic == topic)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FlashCard>> GetCardsByStatusAsync(CardStatus status)
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Where(c => c.Status == status)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FlashCard>> GetCardsByTypeAsync(CardType type)
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Where(c => c.Type == type)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllTopicsAsync()
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Select(c => c.Topic)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<FlashCard> AddCardAsync(FlashCard card)
        {
            using var context = new FlashCardContext();
            context.FlashCards.Add(card);
            await context.SaveChangesAsync();
            
            CardsChanged?.Invoke();
            
            return card;
        }

        public async Task<FlashCard> UpdateCardAsync(FlashCard card)
        {
            using var context = new FlashCardContext();
            context.FlashCards.Update(card);
            await context.SaveChangesAsync();
            
            CardsChanged?.Invoke();
            
            return card;
        }

        public async Task DeleteCardAsync(int cardId)
        {
            using var context = new FlashCardContext();
            var card = await context.FlashCards.FindAsync(cardId);
            if (card != null)
            {
                context.FlashCards.Remove(card);
                await context.SaveChangesAsync();
                
                CardsChanged?.Invoke();
            }
        }

        public async Task UpdateCardStatusAsync(int cardId, CardStatus status)
        {
            using var context = new FlashCardContext();
            var card = await context.FlashCards.FindAsync(cardId);
            if (card != null)
            {
                card.Status = status;
                card.LastViewedAt = DateTime.Now;
                card.ViewCount++;
                await context.SaveChangesAsync();
                
                CardsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Получает leech-карточки (сложные карточки)
        /// </summary>
        public async Task<List<FlashCard>> GetLeechCardsAsync()
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Where(c => c.IsLeech)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Получает приостановленные карточки
        /// </summary>
        public async Task<List<FlashCard>> GetSuspendedCardsAsync()
        {
            using var context = new FlashCardContext();
            return await context.FlashCards
                .Where(c => c.IsSuspended)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Приостанавливает карточку
        /// </summary>
        public async Task SuspendCardAsync(int cardId)
        {
            using var context = new FlashCardContext();
            var card = await context.FlashCards.FindAsync(cardId);
            if (card != null)
            {
                card.IsSuspended = true;
                await context.SaveChangesAsync();
                
                CardsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Возобновляет карточку
        /// </summary>
        public async Task UnsuspendCardAsync(int cardId)
        {
            using var context = new FlashCardContext();
            var card = await context.FlashCards.FindAsync(cardId);
            if (card != null)
            {
                card.IsSuspended = false;
                await context.SaveChangesAsync();
                
                CardsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Удаляет все карточки по теме
        /// </summary>
        public async Task DeleteCardsByTopicAsync(string topic)
        {
            using var context = new FlashCardContext();
            var cards = await context.FlashCards
                .Where(c => c.Topic == topic)
                .ToListAsync();
            
            context.FlashCards.RemoveRange(cards);
            await context.SaveChangesAsync();
            
            CardsChanged?.Invoke();
        }

        /// <summary>
        /// Получает статистику по карточкам
        /// </summary>
        public async Task<CardStatistics> GetStatisticsAsync()
        {
            using var context = new FlashCardContext();
            var allCards = await context.FlashCards.ToListAsync();
            
            return new CardStatistics
            {
                TotalCards = allCards.Count,
                NewCards = allCards.Count(c => c.Type == CardType.New),
                LearningCards = allCards.Count(c => c.Type == CardType.Learning),
                ReviewCards = allCards.Count(c => c.Type == CardType.Review),
                RelearningCards = allCards.Count(c => c.Type == CardType.Relearning),
                LeechCards = allCards.Count(c => c.IsLeech),
                SuspendedCards = allCards.Count(c => c.IsSuspended),
                Topics = allCards.Select(c => c.Topic).Distinct().Count()
            };
        }

        /// <summary>
        /// Применяет результат интервального повторения к карточке (SM-2/QuickMind).
        /// УСТАРЕЛО: Используйте SpacedRepetitionService вместо этого метода
        /// </summary>
        [Obsolete("Используйте SpacedRepetitionService.ApplyResultAsync вместо этого метода")]
        public async Task ApplySpacedRepetitionResultAsync(int cardId, int quality)
        {
            using var context = new FlashCardContext();
            var card = await context.FlashCards.FindAsync(cardId);
            if (card == null) return;

            // SM-2 алгоритм (адаптирован для QuickMind)
            // quality: 0-5 (0=не помню, 5=очень легко)
            if (quality < 3)
            {
                card.Repetitions = 0;
                card.Interval = 1;
            }
            else
            {
                card.Repetitions++;
                switch (card.Repetitions)
                {
                    case 1:
                        card.Interval = 1;
                        break;
                    case 2:
                        card.Interval = 6;
                        break;
                    default:
                        card.Interval = (int)Math.Round(card.Interval * card.EaseFactor);
                        break;
                }
            }
            // Корректировка коэффициента легкости
            card.EaseFactor = Math.Max(1.3, card.EaseFactor + 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;

            // Смена статуса для канбан-доски
            if (quality <= 3)
                card.Status = CardStatus.Learning;
            else
                card.Status = CardStatus.Review;

            await context.SaveChangesAsync();
            CardsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Статистика по карточкам
    /// </summary>
    public class CardStatistics
    {
        public int TotalCards { get; set; }
        public int NewCards { get; set; }
        public int LearningCards { get; set; }
        public int ReviewCards { get; set; }
        public int RelearningCards { get; set; }
        public int LeechCards { get; set; }
        public int SuspendedCards { get; set; }
        public int Topics { get; set; }
    }
} 