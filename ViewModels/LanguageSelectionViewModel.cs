using QuickMind.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuickMind.ViewModels
{
    public class LanguageSelectionViewModel : ViewModelBase
    {
        private readonly LocalizationService _localizationService;
        private LanguageInfo _selectedLanguage;

        public LanguageSelectionViewModel()
        {
            _localizationService = LocalizationService.Instance;
            
            AvailableLanguages = new ObservableCollection<LanguageInfo>
            {
                new LanguageInfo("en", "English", "🇺🇸"),
                new LanguageInfo("ru", "Русский", "🇷🇺"),
                new LanguageInfo("zh-CN", "中文", "🇨🇳")
            };

            _selectedLanguage = AvailableLanguages.FirstOrDefault(l => l.Code == _localizationService.CurrentLanguage) 
                               ?? AvailableLanguages.First();

            InitializeCommands();
        }

        public ObservableCollection<LanguageInfo> AvailableLanguages { get; }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string SelectLanguageTitle => GetLocalizedString("SelectLanguage");
        public string WelcomeMessage => GetLocalizedString("WelcomeMessage");
        public string LanguageDescription => GetLocalizedString("LanguageDescription");
        public string ContinueLabel => GetLocalizedString("Continue");

        public ICommand SelectLanguageCommand { get; private set; } = null!;

        public event Action<string>? LanguageSelected;

        private void InitializeCommands()
        {
            SelectLanguageCommand = new RelayCommand(SelectLanguage);
            SelectSpecificLanguageCommand = new RelayCommand<object>(SelectSpecificLanguage);
        }

        private void SelectLanguage()
        {
            if (SelectedLanguage != null)
            {
                _localizationService.SetLanguage(SelectedLanguage.Code);
                LanguageSelected?.Invoke(SelectedLanguage.Code);
            }
        }

        public ICommand SelectSpecificLanguageCommand { get; private set; } = null!;

        private void SelectSpecificLanguage(object? parameter)
        {
            if (parameter is LanguageInfo languageInfo)
            {
                SelectedLanguage = languageInfo;
                _localizationService.SetLanguage(languageInfo.Code);
                LanguageSelected?.Invoke(languageInfo.Code);
            }
        }

        private string GetLocalizedString(string key)
        {
            try
            {
                return _localizationService.GetString(key);
            }
            catch
            {
                return key switch
                {
                    "SelectLanguage" => "Select Language",
                    "WelcomeMessage" => "Welcome to QuickMind!",
                    "LanguageDescription" => "Please select your preferred language:",
                    "Continue" => "Continue",
                    _ => key
                };
            }
        }
    }

    public class LanguageInfo
    {
        public LanguageInfo(string code, string name, string flag)
        {
            Code = code;
            Name = name;
            Flag = flag;
        }

        public string Code { get; }
        public string Name { get; }
        public string Flag { get; }
        public string DisplayName => $"{Flag} {Name}";
    }
} 