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
        
        public ImportDialog(AnkiImportService importService)
        {
            InitializeComponent();
            _importService = importService;
            
            // Подписываемся на изменения в полях для активации кнопки
            FilePathTextBox.TextChanged += UpdateImportButtonState;
            TopicTextBox.TextChanged += UpdateImportButtonState;
            
            // Устанавливаем начальное состояние кнопки
            UpdateImportButtonState(null, null);
        }
        
        private void UpdateImportButtonState(object? sender, EventArgs? e)
        {
            ImportButton.IsEnabled = !string.IsNullOrEmpty(FilePathTextBox.Text) && 
                                   !string.IsNullOrEmpty(TopicTextBox.Text?.Trim());
        }
        
        private async void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл для импорта",
                AllowMultiple = false
            };
            
            // Устанавливаем фильтры в зависимости от выбранного формата
            var formatIndex = FormatComboBox.SelectedIndex;
            switch (formatIndex)
            {
                case 0: // CSV
                    dialog.Filters.Add(new FileDialogFilter { Name = "CSV файлы", Extensions = { "csv" } });
                    break;
                case 1: // Текстовый
                    dialog.Filters.Add(new FileDialogFilter { Name = "Текстовые файлы", Extensions = { "txt" } });
                    break;
                case 2: // JSON
                    dialog.Filters.Add(new FileDialogFilter { Name = "JSON файлы", Extensions = { "json" } });
                    break;
                case 3: // APKG
                    dialog.Filters.Add(new FileDialogFilter { Name = "Anki пакеты", Extensions = { "apkg" } });
                    break;
            }
            
            dialog.Filters.Add(new FileDialogFilter { Name = "Все файлы", Extensions = { "*" } });
            
            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                FilePathTextBox.Text = result[0];
            }
        }
        
        private async void OnImportClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePathTextBox.Text))
                return;
                
            if (!File.Exists(FilePathTextBox.Text))
            {
                ResultTextBlock.Text = "Ошибка: Файл не найден";
                return;
            }
            
            var topicName = TopicTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                ResultTextBlock.Text = "Ошибка: Введите название темы";
                return;
            }
            
            try
            {
                ResultTextBlock.Text = "Импорт в процессе...";
                
                // Используем универсальный метод импорта с указанием темы
                var result = await _importService.ImportFromFileAsync(FilePathTextBox.Text, topicName);
                
                if (result?.Success == true)
                {
                    var message = $"Импорт завершен успешно!\n\nИмпортировано карточек: {result.ImportedCards}";
                    
                    if (result.Errors.Count > 0)
                    {
                        message += $"\n\nОшибки ({result.Errors.Count}):\n";
                        foreach (var error in result.Errors.Take(5)) // Показываем только первые 5 ошибок
                        {
                            message += $"• {error}\n";
                        }
                        if (result.Errors.Count > 5)
                        {
                            message += $"• ... и еще {result.Errors.Count - 5} ошибок\n";
                        }
                    }
                    
                    ResultTextBlock.Text = message;
                }
                else
                {
                    ResultTextBlock.Text = $"Ошибка импорта: {result?.ErrorMessage ?? "Неизвестная ошибка"}";
                }
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }
        
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}