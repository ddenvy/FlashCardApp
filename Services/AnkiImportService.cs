using QuickMind.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace QuickMind.Services
{
    public class AnkiImportService
    {
        private readonly CardService _cardService;
        
        public AnkiImportService(CardService cardService)
        {
            _cardService = cardService;
        }
        
        /// <summary>
        /// Универсальный метод импорта из файла (определяет формат по расширению)
        /// </summary>
        public async Task<ImportResult> ImportFromFileAsync(string filePath, string? topicOverride = null)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            try
            {
                ImportResult result = extension switch
                {
                    ".apkg" => await ImportFromAnkiPackageAsync(filePath),
                    ".json" => await ImportFromJsonAsync(filePath),
                    ".csv" => await ImportFromCsvAsync(filePath),
                    ".txt" => await ImportFromTextFileAsync(filePath),
                    _ => new ImportResult
                    {
                        Success = false,
                        ErrorMessage = $"Неподдерживаемый формат файла: {extension}",
                        ImportedCards = 0
                    }
                };
                
                // Если указана тема для переопределения, обновляем все импортированные карточки
                if (!string.IsNullOrEmpty(topicOverride) && result.Success)
                {
                    await UpdateImportedCardsTopic(topicOverride);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ImportedCards = 0
                };
            }
        }
        
        private async Task UpdateImportedCardsTopic(string newTopic)
        {
            var recentCards = await _cardService.GetCardsByTopicAsync("Импортированные");
            foreach (var card in recentCards.Where(c => c.CreatedAt > DateTime.Now.AddMinutes(-5)))
            {
                card.Topic = newTopic;
                await _cardService.UpdateCardAsync(card);
            }
        }
        
        /// <summary>
        /// Импортирует колоду из файла Anki (.apkg)
        /// </summary>
        public async Task<ImportResult> ImportFromAnkiPackageAsync(string filePath)
        {
            try
            {
                // TODO: Реализовать распаковку .apkg файла
                // .apkg - это zip-архив с SQLite базой данных
                throw new NotImplementedException("Импорт из .apkg файлов пока не реализован");
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ImportedCards = 0
                };
            }
        }
        
        /// <summary>
        /// Импортирует колоду из JSON файла (экспорт из Anki)
        /// </summary>
        public async Task<ImportResult> ImportFromJsonAsync(string filePath)
        {
            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var ankiData = JsonSerializer.Deserialize<AnkiExportData>(jsonContent);
                
                if (ankiData?.Notes == null)
                {
                    return new ImportResult
                    {
                        Success = false,
                        ErrorMessage = "Неверный формат JSON файла",
                        ImportedCards = 0
                    };
                }
                
                var importedCards = 0;
                var errors = new List<string>();
                
                foreach (var note in ankiData.Notes)
                {
                    try
                    {
                        var card = ConvertAnkiNoteToFlashCard(note);
                        await _cardService.AddCardAsync(card);
                        importedCards++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ошибка при импорте карточки {note.Id}: {ex.Message}");
                    }
                }
                
                return new ImportResult
                {
                    Success = true,
                    ImportedCards = importedCards,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ImportedCards = 0
                };
            }
        }
        
        /// <summary>
        /// Импортирует колоду из CSV файла (универсальный формат)
        /// </summary>
        public async Task<ImportResult> ImportFromCsvAsync(string filePath, string delimiter = ",")
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length < 2) // Заголовок + хотя бы одна строка
                {
                    return new ImportResult
                    {
                        Success = false,
                        ErrorMessage = "Файл слишком короткий",
                        ImportedCards = 0
                    };
                }
                
                var importedCards = 0;
                var errors = new List<string>();
                
                // Пропускаем заголовок
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var parts = lines[i].Split(delimiter);
                        if (parts.Length >= 2)
                        {
                            var card = new FlashCard
                            {
                                Question = parts[0].Trim('"'),
                                Answer = parts[1].Trim('"'),
                                Topic = parts.Length > 2 ? parts[2].Trim('"') : "Импортированные",
                                Type = CardType.New,
                                Status = CardStatus.New,
                                CreatedAt = DateTime.Now,
                                DueDate = DateTime.Today
                            };
                            
                            await _cardService.AddCardAsync(card);
                            importedCards++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ошибка в строке {i + 1}: {ex.Message}");
                    }
                }
                
                return new ImportResult
                {
                    Success = true,
                    ImportedCards = importedCards,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ImportedCards = 0
                };
            }
        }
        
        /// <summary>
        /// Импортирует колоду из текстового файла (формат: вопрос - ответ)
        /// </summary>
        public async Task<ImportResult> ImportFromTextFileAsync(string filePath, string separator = "-")
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var importedCards = 0;
                var errors = new List<string>();
                
                foreach (var line in lines)
                {
                    try
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrEmpty(trimmedLine)) continue;
                        
                        var separatorIndex = trimmedLine.IndexOf(separator);
                        if (separatorIndex == -1)
                        {
                            errors.Add($"Строка не содержит разделитель '{separator}': {trimmedLine}");
                            continue;
                        }
                        
                        var question = trimmedLine.Substring(0, separatorIndex).Trim();
                        var answer = trimmedLine.Substring(separatorIndex + separator.Length).Trim();
                        
                        if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
                        {
                            errors.Add($"Пустой вопрос или ответ: {trimmedLine}");
                            continue;
                        }
                        
                        var card = new FlashCard
                        {
                            Question = question,
                            Answer = answer,
                            Topic = "Импортированные",
                            Type = CardType.New,
                            Status = CardStatus.New,
                            CreatedAt = DateTime.Now,
                            DueDate = DateTime.Today
                        };
                        
                        await _cardService.AddCardAsync(card);
                        importedCards++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ошибка при обработке строки: {ex.Message}");
                    }
                }
                
                return new ImportResult
                {
                    Success = true,
                    ImportedCards = importedCards,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ImportedCards = 0
                };
            }
        }
        
        /// <summary>
        /// Конвертирует заметку Anki в карточку QuickMind
        /// </summary>
        private FlashCard ConvertAnkiNoteToFlashCard(AnkiNote note)
        {
            var card = new FlashCard
            {
                Question = note.Fields[0], // Первое поле - вопрос
                Answer = note.Fields[1],   // Второе поле - ответ
                Topic = GetTopicFromTags(note.Tags),
                Type = CardType.New,
                Status = CardStatus.New,
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(note.Created).DateTime,
                DueDate = DateTime.Today,
                EaseFactor = 2.5,
                Interval = 1,
                Repetitions = 0
            };
            
            return card;
        }
        
        /// <summary>
        /// Извлекает тему из тегов Anki
        /// </summary>
        private string GetTopicFromTags(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return "Импортированные";
            
            // Ищем тег, который может быть темой
            foreach (var tag in tags)
            {
                if (!string.IsNullOrEmpty(tag) && !tag.StartsWith("leech"))
                {
                    return tag;
                }
            }
            
            return "Импортированные";
        }
    }
    
    /// <summary>
    /// Результат импорта
    /// </summary>
    public class ImportResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int ImportedCards { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// Структура данных экспорта Anki
    /// </summary>
    public class AnkiExportData
    {
        public AnkiNote[]? Notes { get; set; }
    }
    
    /// <summary>
    /// Заметка Anki
    /// </summary>
    public class AnkiNote
    {
        public long Id { get; set; }
        public string[] Fields { get; set; } = Array.Empty<string>();
        public string[] Tags { get; set; } = Array.Empty<string>();
        public long Created { get; set; }
        public long Modified { get; set; }
    }
} 