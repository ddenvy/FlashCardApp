using FlashCardApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashCardApp.Services
{
    public class CardService
    {
        public async Task InitializeDatabaseAsync()
        {
            using var context = new FlashCardContext();
            await context.Database.EnsureCreatedAsync();
            
            // База данных создается пустой - пользователь может добавить свои карточки
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
            return card;
        }

        public async Task<FlashCard> UpdateCardAsync(FlashCard card)
        {
            using var context = new FlashCardContext();
            context.FlashCards.Update(card);
            await context.SaveChangesAsync();
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
            }
        }
    }
} 