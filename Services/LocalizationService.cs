using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace QuickMind.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private static LocalizationService? _instance;
        private static readonly object _lock = new object();
        private CultureInfo _currentCulture;
        private readonly string _settingsPath;

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
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "QuickMind");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
                
            _settingsPath = Path.Combine(appDataPath, "settings.json");
            
            var savedLanguage = LoadLanguageFromSettings();
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                _currentCulture = CultureInfo.GetCultureInfo(savedLanguage);
                SetLanguage(savedLanguage);
            }
            else
            {
                _currentCulture = CultureInfo.GetCultureInfo("en");
                SetLanguage("en");
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
                    OnPropertyChanged(nameof(CurrentLanguage));
                    LanguageChanged?.Invoke();
                }
            }
        }

        public string CurrentLanguage => _currentCulture.Name;

        public bool IsFirstRun => !File.Exists(_settingsPath) || IsLanguageNotConfigured();
        
        private bool IsLanguageNotConfigured()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                    return true;
                    
                var content = File.ReadAllText(_settingsPath);
                if (string.IsNullOrWhiteSpace(content))
                    return true;
                    
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                return settings == null || !settings.ContainsKey("Language") || string.IsNullOrWhiteSpace(settings["Language"]);
            }
            catch
            {
                return true; // Если ошибка при чтении - считаем что нужно показать диалог
            }
        }

        public event Action? LanguageChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<LanguageItem> AvailableLanguages { get; } = new()
        {
            new LanguageItem { Code = "en", Name = "English", NativeName = "English" },
            new LanguageItem { Code = "ru", Name = "Русский", NativeName = "Русский" },
            new LanguageItem { Code = "zh-CN", Name = "Chinese (Simplified)", NativeName = "中文简体" }
        };

        public void SetLanguage(string languageCode)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(languageCode);
                CurrentCulture = culture;
                
                SaveLanguageToSettings(languageCode);
            }
            catch (CultureNotFoundException)
            {
                CurrentCulture = CultureInfo.GetCultureInfo("en");
            }
        }

        public string GetString(string key)
        {
            return GetLocalizedString(key);
        }

        public string GetString(string key, params object[] args)
        {
            try
            {
                var format = GetLocalizedString(key);
                return string.Format(format, args);
            }
            catch
            {
                return key;
            }
        }

        private string GetLocalizedString(string key)
        {
            var strings = CurrentCulture.Name switch
            {
                "ru" => GetRussianStrings(),
                "zh-CN" => GetChineseStrings(),
                _ => GetEnglishStrings()
            };

            return strings.TryGetValue(key, out var value) ? value : key;
        }

        private Dictionary<string, string> GetEnglishStrings()
        {
            return new Dictionary<string, string>
            {
                { "All", "All" },
                { "New", "New" },
                { "Learning", "Learning" },
                { "Known", "Known" },
                { "AddCard", "Add Card" },
                { "EditCard", "Edit Card" },
                { "DeleteCard", "Delete Card" },
                { "StudyMode", "Study Mode" },
                { "Question", "Question" },
                { "Answer", "Answer" },
                { "Topic", "Topic" },
                { "Status", "Status" },
                { "Save", "Save" },
                { "Cancel", "Cancel" },
                { "Close", "Close" },
                { "Edit", "Edit" },
                { "Delete", "Delete" },
                { "ShowAnswer", "Show Answer" },
                { "Know", "Know" },
                { "DontKnow", "Don't Know" },
                { "Next", "Next" },
                { "Previous", "Previous" },
                { "Restart", "Restart" },
                { "NoCardsToStudy", "No cards to study" },
                { "AddCardsOrSelectTopic", "Add cards or select a different topic" },
                { "ClickToShowAnswer", "Click to show answer" },
                { "RateKnowledge", "Rate your knowledge" },
                { "NewTopic", "New Topic" },
                { "Preview", "Preview" },
                { "OrCreateNew", "or create new →" },
                { "SelectLanguage", "Select Language" },
                { "WelcomeMessage", "Welcome to QuickMind!" },
                { "LanguageDescription", "Please select your preferred language:" },
                { "Continue", "Continue" },
                { "DeleteConfirmation", "Delete card?\n\nQuestion: {0}" },
                { "DeleteConfirmationTitle", "Delete Confirmation" },
                { "Error", "Error" },
                { "DeleteError", "Error deleting card: {0}" }
            };
        }

        private Dictionary<string, string> GetRussianStrings()
        {
            return new Dictionary<string, string>
            {
                { "All", "Все темы" },
                { "New", "Новые" },
                { "Learning", "Изучаю" },
                { "Known", "Знаю" },
                { "AddCard", "Добавить карточку" },
                { "EditCard", "Редактировать карточку" },
                { "DeleteCard", "Удалить карточку" },
                { "StudyMode", "Режим обучения" },
                { "Question", "Вопрос" },
                { "Answer", "Ответ" },
                { "Topic", "Тема" },
                { "Status", "Статус" },
                { "Save", "Сохранить" },
                { "Cancel", "Отмена" },
                { "Close", "Закрыть" },
                { "Edit", "Изменить" },
                { "Delete", "Удалить" },
                { "ShowAnswer", "Показать ответ" },
                { "Know", "Знаю" },
                { "DontKnow", "Не знаю" },
                { "Next", "Следующая" },
                { "Previous", "Предыдущая" },
                { "Restart", "Начать сначала" },
                { "NoCardsToStudy", "Нет карточек для изучения" },
                { "AddCardsOrSelectTopic", "Добавьте карточки или выберите другую тему" },
                { "ClickToShowAnswer", "Нажмите, чтобы показать ответ" },
                { "RateKnowledge", "Оцените свои знания" },
                { "NewTopic", "Новая тема" },
                { "Preview", "Предварительный просмотр" },
                { "OrCreateNew", "или создать новую →" },
                { "SelectLanguage", "Выбор языка" },
                { "WelcomeMessage", "Добро пожаловать в QuickMind!" },
                { "LanguageDescription", "Пожалуйста, выберите предпочитаемый язык:" },
                { "Continue", "Продолжить" },
                { "DeleteConfirmation", "Удалить карточку?\n\nВопрос: {0}" },
                { "DeleteConfirmationTitle", "Подтверждение удаления" },
                { "Error", "Ошибка" },
                { "DeleteError", "Ошибка при удалении карточки: {0}" }
            };
        }

        private Dictionary<string, string> GetChineseStrings()
        {
            return new Dictionary<string, string>
            {
                { "All", "全部" },
                { "New", "新的" },
                { "Learning", "学习中" },
                { "Known", "已掌握" },
                { "AddCard", "添加卡片" },
                { "EditCard", "编辑卡片" },
                { "DeleteCard", "删除卡片" },
                { "StudyMode", "学习模式" },
                { "Question", "问题" },
                { "Answer", "答案" },
                { "Topic", "主题" },
                { "Status", "状态" },
                { "Save", "保存" },
                { "Cancel", "取消" },
                { "Close", "关闭" },
                { "Edit", "编辑" },
                { "Delete", "删除" },
                { "ShowAnswer", "显示答案" },
                { "Know", "已掌握" },
                { "DontKnow", "不会" },
                { "Next", "下一个" },
                { "Previous", "上一个" },
                { "Restart", "重新开始" },
                { "NoCardsToStudy", "没有卡片可学习" },
                { "AddCardsOrSelectTopic", "添加卡片或选择其他主题" },
                { "ClickToShowAnswer", "点击显示答案" },
                { "RateKnowledge", "评估您的知识" },
                { "NewTopic", "新主题" },
                { "Preview", "预览" },
                { "OrCreateNew", "或创建新的 →" },
                { "SelectLanguage", "选择语言" },
                { "WelcomeMessage", "欢迎使用 QuickMind！" },
                { "LanguageDescription", "请选择您的首选语言：" },
                { "Continue", "继续" },
                { "DeleteConfirmation", "删除卡片吗？\n\n问题：{0}" },
                { "DeleteConfirmationTitle", "删除确认" },
                { "Error", "错误" },
                { "DeleteError", "删除卡片时出错：{0}" }
            };
        }

        private void SaveLanguageToSettings(string languageCode)
        {
            try
            {
                var settings = new { Language = languageCode };
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
            }
        }

        private string? LoadLanguageFromSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    return settings?["Language"]?.ToString();
                }
            }
            catch
            {
            }
            return null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LanguageItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NativeName { get; set; } = string.Empty;

        public override string ToString() => NativeName;
    }
} 