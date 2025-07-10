using QuickMind.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickMind.Services
{
    public class ImportService
    {
        private readonly CardService _cardService;

        public ImportService(CardService cardService)
        {
            _cardService = cardService;
        }

        public async Task<ImportResult> ImportFromFileAsync(string filePath, string topic)
        {
            var result = new ImportResult();
            
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var cards = new List<FlashCard>();
                
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    var parts = line.Split('\t');
                    if (parts.Length >= 2)
                    {
                        var question = parts[0].Trim();
                        var answer = parts[1].Trim();
                        
                        if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
                        {
                            var card = new FlashCard
                            {
                                Question = question,
                                Answer = answer,
                                Topic = topic,
                                Status = CardStatus.New,
                                CreatedAt = DateTime.Now
                            };
                            
                            cards.Add(card);
                        }
                    }
                }
                
                // Добавляем карточки в базу данных
                foreach (var card in cards)
                {
                    await _cardService.AddCardAsync(card);
                }
                
                result.Success = true;
                result.ImportedCount = cards.Count;
                result.Message = $"Успешно импортировано {cards.Count} карточек в тему '{topic}'";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ошибка при импорте: {ex.Message}";
            }
            
            return result;
        }

        public async Task<ImportResult> ImportFromCsvAsync(string filePath, string topic)
        {
            var result = new ImportResult();
            
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var cards = new List<FlashCard>();
                
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    // Простой парсинг CSV (разделитель - запятая)
                    var parts = ParseCsvLine(line);
                    if (parts.Length >= 2)
                    {
                        var question = parts[0].Trim();
                        var answer = parts[1].Trim();
                        
                        if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
                        {
                            var card = new FlashCard
                            {
                                Question = question,
                                Answer = answer,
                                Topic = topic,
                                Status = CardStatus.New,
                                CreatedAt = DateTime.Now
                            };
                            
                            cards.Add(card);
                        }
                    }
                }
                
                // Добавляем карточки в базу данных
                foreach (var card in cards)
                {
                    await _cardService.AddCardAsync(card);
                }
                
                result.Success = true;
                result.ImportedCount = cards.Count;
                result.Message = $"Успешно импортировано {cards.Count} карточек в тему '{topic}'";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ошибка при импорте: {ex.Message}";
            }
            
            return result;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += ch;
                }
            }
            
            result.Add(current);
            return result.ToArray();
        }
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public int ImportedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}