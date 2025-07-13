using QuickMind.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using ClosedXML.Excel;

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
                    ".xlsx" => await ImportFromXlsxAsync(filePath),
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
            // Ограничиваем длину названия темы 20 символами
            if (newTopic.Length > 20)
            {
                newTopic = newTopic.Substring(0, 20);
            }
            
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
        /// Формат: Первый столбец = Вопрос, Второй столбец = Ответ
        /// </summary>
        public async Task<ImportResult> ImportFromCsvAsync(string filePath, string delimiter = ",")
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length < 1)
                {
                    return new ImportResult
                    {
                        Success = false,
                        ErrorMessage = "Файл пуст",
                        ImportedCards = 0
                    };
                }
                
                var importedCards = 0;
                var errors = new List<string>();
                
                // Определяем, есть ли заголовок (если первая строка содержит "вопрос" или "question")
                var startIndex = 0;
                var firstLine = lines[0].ToLower();
                if (firstLine.Contains("вопрос") || firstLine.Contains("question") || 
                    firstLine.Contains("тема") || firstLine.Contains("topic") ||
                    firstLine.Contains("карточка") || firstLine.Contains("card"))
                {
                    startIndex = 1; // Пропускаем заголовок
                }
                
                // Импортируем данные
                for (int i = startIndex; i < lines.Length; i++)
                {
                    try
                    {
                        var line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        
                        var parts = line.Split(delimiter);
                        if (parts.Length >= 2)
                        {
                            var question = parts[0].Trim().Trim('"').Trim('\'');
                            var answer = parts[1].Trim().Trim('"').Trim('\'');
                            
                            if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
                            {
                            var card = new FlashCard
                            {
                                    Question = question,   // Первый столбец = Вопрос
                                    Answer = answer,       // Второй столбец = Ответ
                                    Topic = "Импортированные",
                                Type = CardType.New,
                                Status = CardStatus.New,
                                CreatedAt = DateTime.Now,
                                DueDate = DateTime.Today
                            };
                            
                            await _cardService.AddCardAsync(card);
                            importedCards++;
                            }
                            else
                            {
                                errors.Add($"Строка {i + 1}: Пустой вопрос или ответ");
                            }
                        }
                        else
                        {
                            errors.Add($"Строка {i + 1}: Недостаточно столбцов (нужно минимум 2)");
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
        /// Импортирует колоду из XLSX файла (Excel)
        /// Формат: Первый столбец = Вопрос, Второй столбец = Ответ
        /// </summary>
        public async Task<ImportResult> ImportFromXlsxAsync(string filePath)
        {
            try
            {
                var importedCards = 0;
                var errors = new List<string>();
                
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.First();
                
                // Определяем, есть ли заголовок
                var startRow = 1;
                var firstRow = worksheet.FirstRowUsed();
                if (firstRow != null)
                {
                    var cell1 = firstRow.Cell(1).GetString().ToLower();
                    if (cell1.Contains("вопрос") || cell1.Contains("question") || 
                        cell1.Contains("тема") || cell1.Contains("topic") ||
                        cell1.Contains("карточка") || cell1.Contains("card"))
                    {
                        startRow = 2; // Пропускаем заголовок
                    }
                }
                
                // Импортируем данные
                var rows = worksheet.RowsUsed().Skip(startRow - 1);
                foreach (var row in rows)
                {
                    try
                    {
                        var question = row.Cell(1).GetString().Trim();
                        var answer = row.Cell(2).GetString().Trim();
                        
                        if (!string.IsNullOrEmpty(question) && !string.IsNullOrEmpty(answer))
                        {
                            var card = new FlashCard
                            {
                                Question = question,   // Первый столбец = Вопрос
                                Answer = answer,       // Второй столбец = Ответ
                                Topic = "Импортированные",
                                Type = CardType.New,
                                Status = CardStatus.New,
                                CreatedAt = DateTime.Now,
                                DueDate = DateTime.Today
                            };
                            
                            await _cardService.AddCardAsync(card);
                            importedCards++;
                        }
                        else
                        {
                            errors.Add($"Строка {row.RowNumber()}: Пустой вопрос или ответ");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ошибка в строке {row.RowNumber()}: {ex.Message}");
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
                Topic = "Импортированные", // Тема всегда задается пользователем, не из файла
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
