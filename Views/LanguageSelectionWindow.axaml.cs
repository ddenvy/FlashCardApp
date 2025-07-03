using Avalonia.Controls;
using QuickMind.ViewModels;
using QuickMind.Services;
using Avalonia.Interactivity;

namespace QuickMind.Views
{
    public partial class LanguageSelectionWindow : Window
    {
        public LanguageSelectionWindow()
        {
            InitializeComponent();
            
            var viewModel = new LanguageSelectionViewModel();
            DataContext = viewModel;
        }

        private void OnEnglishClick(object? sender, RoutedEventArgs e)
        {
            SelectLanguage("en");
        }

        private void OnRussianClick(object? sender, RoutedEventArgs e)
        {
            SelectLanguage("ru");
        }

        private void OnChineseClick(object? sender, RoutedEventArgs e)
        {
            SelectLanguage("zh-CN");
        }

        private void SelectLanguage(string languageCode)
        {
            try
            {
                var localizationService = LocalizationService.Instance;
                localizationService.SetLanguage(languageCode);
                Close(languageCode);
            }
            catch
            {
                try
                {
                    Close(languageCode);
                }
                catch
                {
                }
            }
        }
    }
} 