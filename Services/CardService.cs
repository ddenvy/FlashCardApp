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
            
            // Добавляем примеры карточек при первом запуске
            if (!await context.FlashCards.AnyAsync())
            {
                await SeedDataAsync(context);
            }
        }

        private async Task SeedDataAsync(FlashCardContext context)
        {
            var sampleCards = new List<FlashCard>
            {
                new FlashCard
                {
                    Question = "Что такое класс в C#?",
                    Answer = "Класс - это шаблон для создания объектов, который определяет свойства и методы.",
                    Topic = "C#"
                },
                new FlashCard
                {
                    Question = "Что такое SELECT в SQL?",
                    Answer = "SELECT - это команда SQL для извлечения данных из одной или нескольких таблиц.",
                    Topic = "SQL"
                },
                new FlashCard
                {
                    Question = "Что такое MVC в ASP.NET?",
                    Answer = "MVC (Model-View-Controller) - это архитектурный паттерн для разделения логики приложения.",
                    Topic = "ASP.NET"
                }
            };

            context.FlashCards.AddRange(sampleCards);
            await context.SaveChangesAsync();
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