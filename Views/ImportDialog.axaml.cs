using Avalonia.Controls;
using Avalonia.Interactivity;
using QuickMind.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace QuickMind.Views
{
    public partial class ImportDialog : Window
    {
        private readonly AnkiImportService _importService;
        private readonly LocalizationService _localizationService;
        
        public ImportDialog(AnkiImportService importService)
        {
            _importService = importService;
            _localizationService = LocalizationService.Instance;
            
            InitializeComponent();
            
            // Устанавливаем тексты согласно текущему языку
            UpdateTexts();
            
            // Подписываемся на изменения в полях для активации кнопки
            FilePathTextBox.TextChanged += UpdateImportButtonState;
            TopicTextBox.TextChanged += UpdateImportButtonState;
            
            // Подписываемся на изменение языка
            _localizationService.LanguageChanged += OnLanguageChanged;
            
            // Устанавливаем начальное состояние кнопки
            UpdateImportButtonState(null, null);
        }
        
        private void UpdateTexts()
        {
            // Обновляем заголовок окна
            Title = _localizationService.GetString("ImportDialogTitle");
            
            // Обновляем основные элементы
            HeaderTextBlock.Text = _localizationService.GetString("ImportDialogHeader");
            
            // Используем базовые переводы для простых подписей
            var currentCulture = _localizationService.CurrentCulture.Name;
            switch (currentCulture)
            {
                case "ru":
                    FormatLabelTextBlock.Text = "Формат файла:";
                    FileLabelTextBlock.Text = "Файл:";
                    SettingsLabelTextBlock.Text = "Настройки:";
                    SeparatorLabelTextBlock.Text = "Разделитель:";
                    break;
                case "zh-CN":
                    FormatLabelTextBlock.Text = "文件格式:";
                    FileLabelTextBlock.Text = "文件:";
                    SettingsLabelTextBlock.Text = "设置:";
                    SeparatorLabelTextBlock.Text = "分隔符:";
                    break;
                default: // en
                    FormatLabelTextBlock.Text = "File format:";
                    FileLabelTextBlock.Text = "File:";
                    SettingsLabelTextBlock.Text = "Settings:";
                    SeparatorLabelTextBlock.Text = "Separator:";
                    break;
            }
            
            TopicLabelTextBlock.Text = _localizationService.GetString("ImportDialogTopic");
            
            // Обновляем watermarks
            FilePathTextBox.Watermark = _localizationService.GetString("ImportDialogFilePathWatermark");
            TopicTextBox.Watermark = _localizationService.GetString("ImportDialogTopicWatermark");
            
            // Обновляем содержимое кнопок
            BrowseButtonControl.Content = _localizationService.GetString("ImportDialogBrowse");
            ImportButton.Content = _localizationService.GetString("Import");
            CancelButtonControl.Content = _localizationService.GetString("ImportDialogCancel");
            
            // Обновляем элементы комбобокса
            switch (currentCulture)
            {
                case "ru":
                    CsvFormatItem.Content = "CSV (разделённые запятыми значения)";
                    XlsxFormatItem.Content = "XLSX (Excel файл)";
                    TxtFormatItem.Content = "TXT (вопрос-ответ)";
                    JsonFormatItem.Content = "JSON (Anki экспорт)";
                    ApkgFormatItem.Content = "APKG (Anki колода)";
                    break;
                case "zh-CN":
                    CsvFormatItem.Content = "CSV (逗号分隔值)";
                    XlsxFormatItem.Content = "XLSX (Excel 文件)";
                    TxtFormatItem.Content = "TXT (问题-答案)";
                    JsonFormatItem.Content = "JSON (Anki 导出)";
                    ApkgFormatItem.Content = "APKG (Anki 牌组)";
                    break;
                default: // en
                    CsvFormatItem.Content = "CSV (comma-separated values)";
                    XlsxFormatItem.Content = "XLSX (Excel file)";
                    TxtFormatItem.Content = "TXT (question-answer)";
                    JsonFormatItem.Content = "JSON (Anki export)";
                    ApkgFormatItem.Content = "APKG (Anki deck)";
                    break;
            }
            
            // Обновляем подсказки формата
            FormatTxtTextBlock.Text = _localizationService.GetString("ImportDialogFormatTxt");
            FormatCsvTextBlock.Text = _localizationService.GetString("ImportDialogFormatCsv");
            
            // Добавляем подсказку для XLSX
            switch (currentCulture)
            {
                case "ru":
                    FormatXlsxTextBlock.Text = "• XLSX файлы: вопрос,ответ";
                    break;
                case "zh-CN":
                    FormatXlsxTextBlock.Text = "• XLSX 文件: 问题,答案";
                    break;
                default: // en
                    FormatXlsxTextBlock.Text = "• XLSX files: question,answer";
                    break;
            }
            
            FormatEachLineTextBlock.Text = _localizationService.GetString("ImportDialogFormatEachLine");
            
            // Обновляем содержимое результирующего текстового блока
            if (string.IsNullOrEmpty(FilePathTextBox.Text))
            {
                switch (currentCulture)
                {
                    case "ru":
                        ResultTextBlock.Text = "Выберите файл и нажмите 'Импортировать'";
                        break;
                    case "zh-CN":
                        ResultTextBlock.Text = "选择文件并点击「导入」";
                        break;
                    default: // en
                        ResultTextBlock.Text = "Select file and click 'Import'";
                        break;
                }
            }
        }
        
        private void OnLanguageChanged()
        {
            UpdateTexts();
        }
        
        private void UpdateImportButtonState(object? sender, EventArgs? e)
        {
            ImportButton.IsEnabled = !string.IsNullOrEmpty(FilePathTextBox.Text) && 
                                   !string.IsNullOrEmpty(TopicTextBox.Text?.Trim());
        }
        
        private async void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            var currentCulture = _localizationService.CurrentCulture.Name;
            
            var dialog = new OpenFileDialog
            {
                Title = currentCulture switch
                {
                    "ru" => "Выберите файл для импорта",
                    "zh-CN" => "选择要导入的文件",
                    _ => "Select file to import"
                },
                AllowMultiple = false
            };
            
            // Устанавливаем фильтры в зависимости от выбранного формата
            var formatIndex = FormatComboBox.SelectedIndex;
            switch (formatIndex)
            {
                case 0: // CSV
                    dialog.Filters.Add(new FileDialogFilter { 
                        Name = currentCulture switch 
                        {
                            "ru" => "CSV файлы",
                            "zh-CN" => "CSV 文件",
                            _ => "CSV files"
                        }, 
                        Extensions = { "csv" } 
                    });
                    break;
                case 1: // Excel
                    dialog.Filters.Add(new FileDialogFilter { 
                        Name = currentCulture switch 
                        {
                            "ru" => "Excel файлы",
                            "zh-CN" => "Excel 文件",
                            _ => "Excel files"
                        }, 
                        Extensions = { "xlsx" } 
                    });
                    break;
                case 2: // Текстовый
                    dialog.Filters.Add(new FileDialogFilter { 
                        Name = currentCulture switch 
                        {
                            "ru" => "Текстовые файлы",
                            "zh-CN" => "文本文件",
                            _ => "Text files"
                        }, 
                        Extensions = { "txt" } 
                    });
                    break;
                case 3: // JSON
                    dialog.Filters.Add(new FileDialogFilter { 
                        Name = currentCulture switch 
                        {
                            "ru" => "JSON файлы",
                            "zh-CN" => "JSON 文件",
                            _ => "JSON files"
                        }, 
                        Extensions = { "json" } 
                    });
                    break;
                case 4: // APKG
                    dialog.Filters.Add(new FileDialogFilter { 
                        Name = currentCulture switch 
                        {
                            "ru" => "Anki пакеты",
                            "zh-CN" => "Anki 包",
                            _ => "Anki packages"
                        }, 
                        Extensions = { "apkg" } 
                    });
                    break;
            }
            
            dialog.Filters.Add(new FileDialogFilter { 
                Name = currentCulture switch 
                {
                    "ru" => "Все файлы",
                    "zh-CN" => "所有文件",
                    _ => "All files"
                }, 
                Extensions = { "*" } 
            });
            
            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                FilePathTextBox.Text = result[0];
            }
        }
        
        private async void OnImportClick(object sender, RoutedEventArgs e)
        {
            var currentCulture = _localizationService.CurrentCulture.Name;
            
            if (string.IsNullOrEmpty(FilePathTextBox.Text))
                return;
                
            if (!File.Exists(FilePathTextBox.Text))
            {
                ResultTextBlock.Text = currentCulture switch
                {
                    "ru" => "Ошибка: Файл не найден",
                    "zh-CN" => "错误：找不到文件",
                    _ => "Error: File not found"
                };
                return;
            }
            
            var topicName = TopicTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                ResultTextBlock.Text = currentCulture switch
                {
                    "ru" => "Ошибка: Введите название темы",
                    "zh-CN" => "错误：请输入主题名称",
                    _ => "Error: Enter topic name"
                };
                return;
            }
            
            try
            {
                ResultTextBlock.Text = currentCulture switch
                {
                    "ru" => "Импорт в процессе...",
                    "zh-CN" => "正在导入...",
                    _ => "Importing..."
                };
                
                // Используем универсальный метод импорта с указанием темы
                var result = await _importService.ImportFromFileAsync(FilePathTextBox.Text, topicName);
                
                if (result?.Success == true)
                {
                    var message = currentCulture switch
                    {
                        "ru" => $"Импорт завершен успешно!\n\nИмпортировано карточек: {result.ImportedCards}",
                        "zh-CN" => $"导入成功完成！\n\n已导入卡片：{result.ImportedCards}",
                        _ => $"Import completed successfully!\n\nImported cards: {result.ImportedCards}"
                    };
                    
                    if (result.Errors.Count > 0)
                    {
                        var errorsHeader = currentCulture switch
                        {
                            "ru" => $"Ошибки ({result.Errors.Count})",
                            "zh-CN" => $"错误 ({result.Errors.Count})",
                            _ => $"Errors ({result.Errors.Count})"
                        };
                        
                        message += $"\n\n{errorsHeader}:\n";
                        foreach (var error in result.Errors.Take(5)) // Показываем только первые 5 ошибок
                        {
                            message += $"• {error}\n";
                        }
                        if (result.Errors.Count > 5)
                        {
                            var moreErrors = currentCulture switch
                            {
                                "ru" => $"... и еще {result.Errors.Count - 5} ошибок",
                                "zh-CN" => $"... 还有 {result.Errors.Count - 5} 个错误",
                                _ => $"... and {result.Errors.Count - 5} more errors"
                            };
                            message += $"• {moreErrors}\n";
                        }
                    }
                    
                    ResultTextBlock.Text = message;
                }
                else
                {
                    var errorPrefix = currentCulture switch
                    {
                        "ru" => "Ошибка импорта",
                        "zh-CN" => "导入错误",
                        _ => "Import error"
                    };
                    
                    var unknownError = currentCulture switch
                    {
                        "ru" => "Неизвестная ошибка",
                        "zh-CN" => "未知错误",
                        _ => "Unknown error"
                    };
                    
                    ResultTextBlock.Text = $"{errorPrefix}: {result?.ErrorMessage ?? unknownError}";
                }
            }
            catch (Exception ex)
            {
                var errorPrefix = currentCulture switch
                {
                    "ru" => "Ошибка",
                    "zh-CN" => "错误",
                    _ => "Error"
                };
                
                ResultTextBlock.Text = $"{errorPrefix}: {ex.Message}";
            }
        }
        
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            base.OnClosed(e);
        }
    }
}