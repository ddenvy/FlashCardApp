# QuickMind Installer Builder

Этот каталог содержит скрипты для создания инсталляторов QuickMind для Windows и macOS.

## 📋 Предварительные требования

### Общие требования
- **.NET 9.0 SDK** или новее
- **Git** (для клонирования репозитория)

### Для Windows
- **Inno Setup 6.0+** - скачать с [официального сайта](https://jrsoftware.org/isdownload.php)
- **PowerShell 5.1+** (обычно уже установлен)

### Для macOS
- **Xcode Command Line Tools**: `xcode-select --install`
- **Bash** (обычно уже установлен)
- Опционально: **create-dmg** для более красивых DMG файлов

## 🚀 Быстрый старт

### Автоматическая сборка всех инсталляторов
```powershell
# Собрать все инсталляторы
.\installer\Build-All.ps1

# Собрать только для Windows
.\installer\Build-All.ps1 -Platform Windows

# Собрать только для macOS
.\installer\Build-All.ps1 -Platform macOS

# Собрать с определенной версией
.\installer\Build-All.ps1 -Version "2.1.0"
```

### Ручная сборка

#### Windows
```powershell
.\installer\Build-Windows-Installer.ps1
```

#### macOS
```bash
./installer/Build-macOS-Installer.sh
```

## 📁 Структура файлов

```
installer/
├── Build-All.ps1              # Универсальный скрипт сборки
├── Build-Windows-Installer.ps1 # Скрипт для Windows
├── Build-macOS-Installer.sh   # Скрипт для macOS
├── QuickMind-Setup.iss        # Inno Setup конфигурация (генерируется)
└── README.md                  # Этот файл

dist/                          # Готовые инсталляторы
├── QuickMind-Setup-v2.0.0.exe # Windows инсталлятор
├── QuickMind-v2.0.0.dmg       # macOS DMG
└── QuickMind.app/             # macOS App Bundle

publish/                       # Скомпилированные файлы
├── win-x64/                   # Windows сборка
└── osx-x64/                   # macOS сборка
```

## 🔧 Настройка

### Изменение версии
Отредактируйте файл `QuickMind.csproj`:
```xml
<Version>2.0.0</Version>
<FileVersion>2.0.0</FileVersion>
```

### Настройка иконки
- **Windows**: Поместите `.ico` файл в `Assets/` и обновите путь в Inno Setup скрипте
- **macOS**: Создайте `.icns` файл и поместите в `Contents/Resources/` app bundle

## 📦 Создаваемые файлы

### Windows
- **QuickMind-Setup-v{version}.exe** - Инсталлятор для Windows
  - Устанавливает приложение в `Program Files`
  - Создает ярлыки в меню "Пуск" и на рабочем столе
  - Поддерживает автоматическое удаление

### macOS
- **QuickMind.app** - Приложение для macOS
- **QuickMind-v{version}.dmg** - DMG образ для установки
  - Содержит app bundle и ссылку на папку Applications
  - Готов для распространения

## 🔐 Подписание и нотаризация

### Windows (опционально)
```powershell
# Подписание (требует сертификат)
signtool sign /f certificate.pfx /p password QuickMind-Setup-v2.0.0.exe
```

### macOS (рекомендуется для распространения)
```bash
# Подписание app bundle
codesign --force --deep --sign "Developer ID Application: Your Name" QuickMind.app

# Нотаризация (требует Apple Developer аккаунт)
xcrun notarytool submit QuickMind-v2.0.0.dmg --keychain-profile "notarytool-profile" --wait

# Скрепление нотаризацией
xcrun stapler staple QuickMind-v2.0.0.dmg
```

## 🐛 Устранение неполадок

### Windows
- **Ошибка "Inno Setup not found"**: Установите Inno Setup и добавьте его в PATH
- **Ошибка сборки**: Проверьте, что .NET SDK установлен правильно
- **Проблемы с подписанием**: Убедитесь, что сертификат действителен

### macOS
- **Ошибка "hdiutil not found"**: Команда должна быть доступна на всех macOS системах
- **Проблемы с иконкой**: Используйте `iconutil` для создания .icns файла
- **Gatekeeper блокирует**: Подпишите и нотаризуйте приложение

## 📝 Примечания

- Инсталляторы создают самодостаточные сборки (не требуют установки .NET)
- Размер инсталлятора: ~60-100 MB (включает .NET Runtime)
- Поддерживаемые версии: Windows 10+ (x64), macOS 10.15+ (x64)

## 🔗 Полезные ссылки

- [Inno Setup Documentation](https://jrsoftware.org/isinfo.php)
- [.NET Publishing Guide](https://docs.microsoft.com/en-us/dotnet/core/deploying/)
- [macOS App Distribution](https://developer.apple.com/documentation/xcode/distributing_your_app_for_beta_testing_and_releases)
- [Code Signing Guide](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution) 