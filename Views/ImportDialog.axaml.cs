using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using QuickMind.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuickMind.Views
{
    public partial class ImportDialog : Window
    {
        private ImportDialogViewModel? _viewModel;

        public ImportDialog()
        {
            InitializeComponent();
        }

        public ImportDialog(ImportDialogViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private async void OnSelectFileClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            var options = new FilePickerOpenOptions
            {
                Title = "Выберите файл с карточками",
                AllowMultiple = false,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Текстовые файлы") { Patterns = new[] { "*.txt", "*.csv" } },
                    new FilePickerFileType("Все файлы") { Patterns = new[] { "*.*" } }
                }
            };

            var result = await StorageProvider.OpenFilePickerAsync(options);
            if (result.Count > 0)
            {
                _viewModel.SelectedFilePath = result[0].Path.LocalPath;
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnImportClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            if (string.IsNullOrEmpty(_viewModel.SelectedFilePath))
            {
                await ShowMessageAsync("Ошибка", "Выберите файл для импорта");
                return;
            }

            if (string.IsNullOrEmpty(_viewModel.SelectedTopic))
            {
                await ShowMessageAsync("Ошибка", "Введите название темы");
                return;
            }

            if (!File.Exists(_viewModel.SelectedFilePath))
            {
                await ShowMessageAsync("Ошибка", "Файл не найден");
                return;
            }

            try
            {
                await _viewModel.ImportAsync();
                
                if (_viewModel.StatusMessage.Contains("Успешно"))
                {
                    await ShowMessageAsync("Успех", _viewModel.StatusMessage);
                    Close();
                }
                else
                {
                    await ShowMessageAsync("Ошибка", _viewModel.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                await ShowMessageAsync("Ошибка", $"Ошибка импорта: {ex.Message}");
            }
        }

        private async Task ShowMessageAsync(string title, string message)
        {
            var okButton = new Button
            {
                Content = "OK",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                MinWidth = 80
            };

            var messageBox = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(30, 30, 30)),
                Content = new Grid
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new StackPanel
                        {
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = message,
                                    Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(255, 255, 255)),
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                    Margin = new Avalonia.Thickness(0, 0, 0, 20)
                                },
                                okButton
                            }
                        }
                    }
                }
            };

            okButton.Click += (s, e) => messageBox.Close();

            await messageBox.ShowDialog(this);
        }
    }
}