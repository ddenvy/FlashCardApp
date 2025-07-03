# 📁 Структура проекта QuickMind

## 🏗️ Архитектура
QuickMind построен на **.NET 9.0** с использованием **Avalonia UI** по паттерну **MVVM**.

## 📂 Структура папок

### 🔧 Основные файлы
```
QuickMind/
├── App.axaml                  # Стили и ресурсы приложения
├── App.axaml.cs              # Логика инициализации приложения
├── Program.cs                # Точка входа в приложение
├── QuickMind.csproj          # Конфигурация проекта
├── ViewLocator.cs            # Связывание View и ViewModel
├── app.manifest              # Манифест для Windows
└── .gitignore                # Игнорируемые файлы Git
```

### 🎯 Модели (Models/)
```
Models/
├── FlashCard.cs              # Модель карточки
└── FlashCardContext.cs       # Контекст базы данных EF Core
```

### 🧠 ViewModels (ViewModels/)
```
ViewModels/
├── MainWindowViewModel.cs           # Главное окно
├── StudyModeViewModel.cs           # Режим изучения
├── AddCardDialogViewModel.cs       # Диалог добавления карточки
├── LanguageSelectionViewModel.cs   # Выбор языка
├── ViewModelBase.cs               # Базовый класс ViewModel
└── RelayCommand.cs                # Команды для MVVM
```

### 🖥️ Представления (Views/)
```
Views/
├── MainWindow.axaml(.cs)           # Главное окно
├── StudyModeWindow.axaml(.cs)      # Окно режима изучения
├── AddCardDialog.axaml(.cs)        # Диалог добавления карточки
└── LanguageSelectionWindow.axaml(.cs) # Окно выбора языка
```

### ⚙️ Сервисы (Services/)
```
Services/
├── CardService.cs            # Работа с карточками
└── LocalizationService.cs   # Многоязычность
```

### 🔄 Конвертеры (Converters/)
```
Converters/
└── LocalizationConverter.cs # Конвертер для локализации
```

### 🎨 Ресурсы (Assets/)
```
Assets/
├── QuickMindLogo.png        # Логотип приложения
└── avalonia-logo.ico        # Иконка Avalonia
```

### 🏗️ Сборка (installer/)
```
installer/
├── Build-All.ps1                    # Сборка для всех платформ
├── Build-Windows-Installer.ps1     # Сборка Windows установщика
├── Build-macOS-Installer.sh        # Сборка macOS DMG
├── QuickMind-Setup.iss             # Конфигурация Inno Setup
└── README.md                       # Документация по сборке
```

### 📦 Дистрибутив (dist/)
```
dist/
├── QuickMind-Setup-v2.1.0.exe          # Windows установщик
├── QuickMind-macOS-ARM64-v2.1.0.zip    # macOS архив
├── RELEASE-NOTES-v2.1.0.md             # Заметки к релизу
├── README.md                           # Инструкции для пользователей
├── README-macOS.md                     # Специфичные инструкции для macOS
└── Install-QuickMind-macOS.sh          # Скрипт установки для macOS
```

### 🤖 GitHub Actions (.github/)
```
.github/
├── workflows/
│   ├── build-macos.yml      # Сборка macOS DMG
│   └── release.yml          # Полный релиз всех платформ
└── README.md               # Документация по CI/CD
```

## 🎨 Дизайн и стили

### Цветовая схема
- **Фон**: #1E1E1E (темный)
- **Панели**: #2A2A2A (темно-серый)
- **Границы**: #333333 (приглушенный серый)
- **Текст**: #FFFFFF (белый) / #B0B0B0 (светло-серый)

### Шрифт
- **Основной**: Corus (используется во всем приложении)

### Компоненты
- **Кнопки**: Единообразные размеры (MinHeight: 36px, Padding: 16,10)
- **Поля ввода**: Консистентные размеры (MinHeight: 36px, Padding: 8px)
- **Заголовки**: Одинаковая ширина в канбан-макете

## 🗄️ База данных
- **Тип**: SQLite с Entity Framework Core
- **Модели**: FlashCard (Question, Answer, Topic, Status, CreatedAt, LastReviewed)
- **Состояния карточек**: New, Learning, Known

## 🌍 Многоязычность
- **Поддерживаемые языки**: English, Russian, Chinese
- **Система**: Собственный LocalizationService с динамической сменой языка

## 🚀 Сборка и развертывание

### Локальная разработка
```bash
dotnet restore
dotnet run
```

### Создание релиза
```bash
# Все платформы
./installer/Build-All.ps1 -Version "2.1.0"

# Только Windows
./installer/Build-Windows-Installer.ps1 -Version "2.1.0"

# Только macOS (требует macOS)
./installer/Build-macOS-Installer.sh
```

### GitHub Actions
Автоматическая сборка при создании тега:
```bash
git tag v2.1.0
git push origin v2.1.0
```

## 🔧 Требования к разработке
- **.NET 9.0 SDK**
- **Visual Studio 2022** или **JetBrains Rider**
- **Git** для контроля версий

## 📝 Файлы конфигурации
- **QuickMind.csproj**: NuGet пакеты и настройки проекта
- **.gitignore**: Исключения для Git (bin/, obj/, publish/, dist/*.app/)
- **app.manifest**: Манифест для Windows (DPI awareness)

---

💡 **Совет**: Для быстрого старта смотрите [README.md](README.md) в корне проекта. 