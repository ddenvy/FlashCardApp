using QuickMind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickMind.Services
{
    public class SpacedRepetitionService
    {
        private readonly CardService _cardService;
        private readonly Random _random = new Random();
        
        // Настройки алгоритма (можно вынести в конфигурацию)
        public class SRSettings
        {
            // Настройки для новых карточек
            public int[] LearningSteps { get; set; } = { 1, 10 }; // в минутах
            public int GraduatingInterval { get; set; } = 1; // в днях
            public int EasyInterval { get; set; } = 4; // в днях
            public double StartingEase { get; set; } = 2.5;
            
            // Настройки для повторений
            public double EasyBonus { get; set; } = 1.3;
            public double HardInterval { get; set; } = 1.2;
            public double IntervalModifier { get; set; } = 1.0;
            public int MaximumInterval { get; set; } = 36500; // ~100 лет
            
            // Настройки для ошибок
            public int[] RelearningSteps { get; set; } = { 10 }; // в минутах
            public double NewIntervalMultiplier { get; set; } = 0.1;
            public int MinimumInterval { get; set; } = 1;
            public int LeechThreshold { get; set; } = 8; // Количество ошибок для leech
        }
        
        public SRSettings Settings { get; set; } = new SRSettings();
        
        public SpacedRepetitionService(CardService cardService)
        {
            _cardService = cardService;
        }
        
        /// <summary>
        /// Применяет результат оценки к карточке с улучшенным алгоритмом
        /// </summary>
        public async Task ApplyResultAsync(FlashCard card, int quality)
        {
            switch (card.Type)
            {
                case CardType.New:
                    await HandleNewCardAsync(card, quality);
                    break;
                case CardType.Learning:
                    await HandleLearningCardAsync(card, quality);
                    break;
                case CardType.Review:
                    await HandleReviewCardAsync(card, quality);
                    break;
                case CardType.Relearning:
                    await HandleRelearningCardAsync(card, quality);
                    break;
            }
            
            await _cardService.UpdateCardAsync(card);
        }
        
        /// <summary>
        /// Обработка новых карточек
        /// </summary>
        private async Task HandleNewCardAsync(FlashCard card, int quality)
        {
            // Переводим в режим обучения
            card.Type = CardType.Learning;
            card.Status = CardStatus.Learning;
            card.LearningStep = 0;
            card.EaseFactor = Settings.StartingEase;
            
            // Устанавливаем время следующего показа
            var nextStep = GetNextLearningStep(card);
            card.LearningDueDate = DateTime.Now.AddMinutes(nextStep);
        }
        
        /// <summary>
        /// Обработка обучаемых карточек
        /// </summary>
        private async Task HandleLearningCardAsync(FlashCard card, int quality)
        {
            switch (quality)
            {
                case 0: // Again
                    card.LearningStep = 0;
                    var againStep = GetNextLearningStep(card);
                    card.LearningDueDate = DateTime.Now.AddMinutes(againStep);
                    break;
                    
                case 1: // Hard
                    // Повторяем текущий шаг с промежуточным интервалом
                    var hardStep = GetHardLearningStep(card);
                    card.LearningDueDate = DateTime.Now.AddMinutes(hardStep);
                    break;
                    
                case 2: // Good
                    card.LearningStep++;
                    if (card.LearningStep >= Settings.LearningSteps.Length)
                    {
                        // Карточка окончила обучение
                        await GraduateCardAsync(card, false);
                    }
                    else
                    {
                        var goodStep = GetNextLearningStep(card);
                        card.LearningDueDate = DateTime.Now.AddMinutes(goodStep);
                    }
                    break;
                    
                case 3: // Easy
                    // Сразу переводим в режим повторения
                    await GraduateCardAsync(card, true);
                    break;
            }
        }
        
        /// <summary>
        /// Обработка карточек на повторении
        /// </summary>
        private async Task HandleReviewCardAsync(FlashCard card, int quality)
        {
            switch (quality)
            {
                case 0: // Again
                    await HandleLapseAsync(card);
                    break;
                    
                case 1: // Hard
                    await HandleHardReviewAsync(card);
                    break;
                    
                case 2: // Good
                    await HandleGoodReviewAsync(card);
                    break;
                    
                case 3: // Easy
                    await HandleEasyReviewAsync(card);
                    break;
            }
        }
        
        /// <summary>
        /// Обработка переобучаемых карточек
        /// </summary>
        private async Task HandleRelearningCardAsync(FlashCard card, int quality)
        {
            switch (quality)
            {
                case 0: // Again
                    card.LearningStep = 0;
                    var againStep = GetNextRelearningStep(card);
                    card.LearningDueDate = DateTime.Now.AddMinutes(againStep);
                    break;
                    
                case 1: // Hard
                    // Повторяем текущий шаг
                    var hardStep = GetHardRelearningStep(card);
                    card.LearningDueDate = DateTime.Now.AddMinutes(hardStep);
                    break;
                    
                case 2: // Good
                    card.LearningStep++;
                    if (card.LearningStep >= Settings.RelearningSteps.Length)
                    {
                        // Возвращаем в режим повторения
                        await ReturnToReviewAsync(card);
                    }
                    else
                    {
                        var goodStep = GetNextRelearningStep(card);
                        card.LearningDueDate = DateTime.Now.AddMinutes(goodStep);
                    }
                    break;
                    
                case 3: // Easy
                    // Сразу возвращаем в режим повторения
                    await ReturnToReviewAsync(card);
                    break;
            }
        }
        
        /// <summary>
        /// Обработка ошибки (lapse)
        /// </summary>
        private async Task HandleLapseAsync(FlashCard card)
        {
            card.Lapses++;
            card.EaseFactor = Math.Max(1.3, card.EaseFactor - 0.2);
            
            // Проверяем, не стала ли карточка leech
            if (card.Lapses >= Settings.LeechThreshold)
            {
                card.IsLeech = true;
                card.IsSuspended = true;
                return;
            }
            
            // Переводим в режим переобучения
            card.Type = CardType.Relearning;
            card.Status = CardStatus.Relearning;
            card.LearningStep = 0;
            
            // Уменьшаем интервал
            card.Interval = Math.Max(Settings.MinimumInterval, 
                (int)(card.Interval * Settings.NewIntervalMultiplier));
            
            var relearningStep = GetNextRelearningStep(card);
            card.LearningDueDate = DateTime.Now.AddMinutes(relearningStep);
        }
        
        /// <summary>
        /// Обработка "сложного" ответа на повторении
        /// </summary>
        private async Task HandleHardReviewAsync(FlashCard card)
        {
            card.EaseFactor = Math.Max(1.3, card.EaseFactor - 0.15);
            
            // Увеличиваем интервал на множитель Hard
            var newInterval = (int)(card.Interval * Settings.HardInterval * Settings.IntervalModifier);
            card.Interval = Math.Min(Settings.MaximumInterval, newInterval);
            
            // Добавляем fuzzing
            card.FuzzFactor = GetFuzzFactor(card.Interval);
            card.Interval += card.FuzzFactor;
            
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;
        }
        
        /// <summary>
        /// Обработка "хорошего" ответа на повторении
        /// </summary>
        private async Task HandleGoodReviewAsync(FlashCard card)
        {
            // Ease factor остается неизменным
            
            // Увеличиваем интервал на ease factor
            var newInterval = (int)(card.Interval * card.EaseFactor * Settings.IntervalModifier);
            card.Interval = Math.Min(Settings.MaximumInterval, newInterval);
            
            // Добавляем fuzzing
            card.FuzzFactor = GetFuzzFactor(card.Interval);
            card.Interval += card.FuzzFactor;
            
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;
        }
        
        /// <summary>
        /// Обработка "легкого" ответа на повторении
        /// </summary>
        private async Task HandleEasyReviewAsync(FlashCard card)
        {
            card.EaseFactor += 0.15;
            
            // Увеличиваем интервал на ease factor * easy bonus
            var newInterval = (int)(card.Interval * card.EaseFactor * Settings.EasyBonus * Settings.IntervalModifier);
            card.Interval = Math.Min(Settings.MaximumInterval, newInterval);
            
            // Добавляем fuzzing
            card.FuzzFactor = GetFuzzFactor(card.Interval);
            card.Interval += card.FuzzFactor;
            
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;
        }
        
        /// <summary>
        /// Окончание обучения карточки
        /// </summary>
        private async Task GraduateCardAsync(FlashCard card, bool isEasy)
        {
            card.Type = CardType.Review;
            card.Status = CardStatus.Review;
            card.LearningDueDate = null;
            
            if (isEasy)
            {
                card.Interval = Settings.EasyInterval;
                card.EaseFactor += 0.15;
            }
            else
            {
                card.Interval = Settings.GraduatingInterval;
            }
            
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;
        }
        
        /// <summary>
        /// Возврат карточки в режим повторения после переобучения
        /// </summary>
        private async Task ReturnToReviewAsync(FlashCard card)
        {
            card.Type = CardType.Review;
            card.Status = CardStatus.Review;
            card.LearningDueDate = null;
            
            // Интервал уже был установлен при lapse
            card.DueDate = DateTime.Today.AddDays(card.Interval);
            card.LastViewedAt = DateTime.Now;
            card.ViewCount++;
        }
        
        /// <summary>
        /// Получает следующий шаг обучения
        /// </summary>
        private int GetNextLearningStep(FlashCard card)
        {
            if (card.LearningStep < Settings.LearningSteps.Length)
            {
                return Settings.LearningSteps[card.LearningStep];
            }
            return Settings.LearningSteps[^1]; // Последний шаг
        }
        
        /// <summary>
        /// Получает промежуточный шаг для "Hard" ответа
        /// </summary>
        private int GetHardLearningStep(FlashCard card)
        {
            var currentStep = GetNextLearningStep(card);
            var nextStep = card.LearningStep + 1 < Settings.LearningSteps.Length 
                ? Settings.LearningSteps[card.LearningStep + 1] 
                : currentStep;
            
            return (currentStep + nextStep) / 2;
        }
        
        /// <summary>
        /// Получает следующий шаг переобучения
        /// </summary>
        private int GetNextRelearningStep(FlashCard card)
        {
            if (card.LearningStep < Settings.RelearningSteps.Length)
            {
                return Settings.RelearningSteps[card.LearningStep];
            }
            return Settings.RelearningSteps[^1];
        }
        
        /// <summary>
        /// Получает промежуточный шаг для "Hard" ответа при переобучении
        /// </summary>
        private int GetHardRelearningStep(FlashCard card)
        {
            var currentStep = GetNextRelearningStep(card);
            var nextStep = card.LearningStep + 1 < Settings.RelearningSteps.Length 
                ? Settings.RelearningSteps[card.LearningStep + 1] 
                : currentStep;
            
            return (currentStep + nextStep) / 2;
        }
        
        /// <summary>
        /// Генерирует случайный сдвиг интервала (fuzzing)
        /// </summary>
        private int GetFuzzFactor(int interval)
        {
            if (interval < 2) return 0;
            if (interval == 2) return _random.Next(0, 2); // 0 или 1
            
            double fuzzPercentage;
            if (interval < 7) fuzzPercentage = 0.25;
            else if (interval < 30) fuzzPercentage = 0.15;
            else fuzzPercentage = 0.05;
            
            var fuzz = Math.Max(1, (int)(interval * fuzzPercentage));
            return _random.Next(-fuzz, fuzz + 1);
        }
        
        /// <summary>
        /// Получает карточки для изучения сегодня
        /// </summary>
        public async Task<List<FlashCard>> GetCardsForTodayAsync()
        {
            var allCards = await _cardService.GetAllCardsAsync();
            var today = DateTime.Today;
            
            return allCards.Where(card => 
                !card.IsSuspended && 
                (card.Type == CardType.New ||
                 (card.Type == CardType.Learning && card.LearningDueDate <= DateTime.Now) ||
                 (card.Type == CardType.Review && card.DueDate <= today) ||
                 (card.Type == CardType.Relearning && card.LearningDueDate <= DateTime.Now))
            ).ToList();
        }
        
        /// <summary>
        /// Получает все карточки для изучения (режим тестирования)
        /// </summary>
        public async Task<List<FlashCard>> GetAllCardsForStudyAsync()
        {
            var allCards = await _cardService.GetAllCardsAsync();
            
            return allCards.Where(card => 
                !card.IsSuspended && 
                (card.Type == CardType.New ||
                 card.Type == CardType.Learning ||
                 card.Type == CardType.Review ||
                 card.Type == CardType.Relearning)
            ).ToList();
        }
        
        /// <summary>
        /// Получает статистику по карточкам
        /// </summary>
        public async Task<SRSStatistics> GetStatisticsAsync()
        {
            var allCards = await _cardService.GetAllCardsAsync();
            
            return new SRSStatistics
            {
                TotalCards = allCards.Count,
                NewCards = allCards.Count(c => c.Type == CardType.New),
                LearningCards = allCards.Count(c => c.Type == CardType.Learning),
                ReviewCards = allCards.Count(c => c.Type == CardType.Review),
                RelearningCards = allCards.Count(c => c.Type == CardType.Relearning),
                LeechCards = allCards.Count(c => c.IsLeech),
                SuspendedCards = allCards.Count(c => c.IsSuspended),
                CardsForToday = (await GetCardsForTodayAsync()).Count
            };
        }
    }
    
    /// <summary>
    /// Статистика по интервальным повторениям
    /// </summary>
    public class SRSStatistics
    {
        public int TotalCards { get; set; }
        public int NewCards { get; set; }
        public int LearningCards { get; set; }
        public int ReviewCards { get; set; }
        public int RelearningCards { get; set; }
        public int LeechCards { get; set; }
        public int SuspendedCards { get; set; }
        public int CardsForToday { get; set; }
    }
} 