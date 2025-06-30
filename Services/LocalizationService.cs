using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;

namespace FlashCardApp.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private static LocalizationService _instance;
        private static readonly object _lock = new object();
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public static LocalizationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new LocalizationService();
                    }
                }
                return _instance;
            }
        }

        private LocalizationService()
        {
            _resourceManager = new ResourceManager("FlashCardApp.Resources.Strings", typeof(LocalizationService).Assembly);
            _currentCulture = CultureInfo.CurrentCulture;
            
            // Устанавливаем язык по умолчанию из настроек или системный
            var savedLanguage = Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                SetLanguage(savedLanguage);
            }
        }

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    Thread.CurrentThread.CurrentCulture = value;
                    Thread.CurrentThread.CurrentUICulture = value;
                    CultureInfo.DefaultThreadCurrentCulture = value;
                    CultureInfo.DefaultThreadCurrentUICulture = value;
                    
                    OnPropertyChanged(nameof(CurrentCulture));
                    LanguageChanged?.Invoke();
                }
            }
        }

        public event Action LanguageChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<LanguageItem> AvailableLanguages { get; } = new()
        {
            new LanguageItem { Code = "ru", Name = "Русский", NativeName = "Русский" },
            new LanguageItem { Code = "en", Name = "English", NativeName = "English" },
            new LanguageItem { Code = "zh-CN", Name = "Chinese (Simplified)", NativeName = "中文(简体)" }
        };

        public void SetLanguage(string languageCode)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(languageCode);
                CurrentCulture = culture;
                
                // Сохраняем выбранный язык в настройках
                Properties.Settings.Default.Language = languageCode;
                Properties.Settings.Default.Save();
            }
            catch (CultureNotFoundException)
            {
                // Если культура не найдена, используем английский как запасной вариант
                CurrentCulture = CultureInfo.GetCultureInfo("en");
            }
        }

        public string GetString(string key)
        {
            try
            {
                return _resourceManager.GetString(key, CurrentCulture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        public string GetString(string key, params object[] args)
        {
            try
            {
                var format = _resourceManager.GetString(key, CurrentCulture);
                return format != null ? string.Format(format, args) : key;
            }
            catch
            {
                return key;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LanguageItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }

        public override string ToString() => NativeName;
    }

    // Расширение разметки для использования в XAML
    public class LocalizeExtension : System.Windows.Markup.MarkupExtension
    {
        private readonly string _key;

        public LocalizeExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new System.Windows.Data.Binding("CurrentCulture")
            {
                Source = LocalizationService.Instance,
                Converter = new LocalizationConverter(),
                ConverterParameter = _key
            };

            return binding.ProvideValue(serviceProvider);
        }
    }

    // Конвертер для локализации
    public class LocalizationConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter?.ToString() is string key)
            {
                return LocalizationService.Instance.GetString(key);
            }
            return parameter?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 