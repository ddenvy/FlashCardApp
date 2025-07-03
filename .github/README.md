# GitHub Actions для QuickMind

Этот репозиторий содержит автоматические workflows для сборки и релиза приложения QuickMind на разных платформах.

## Workflows

### 1. `build-macos.yml` - Сборка macOS
**Назначение**: Создает DMG файлы для macOS (Intel и Apple Silicon)

**Запуск**:
- Автоматически при создании тега версии (`v*`)
- Вручную через GitHub Actions UI
- При pull request в main ветку

**Что делает**:
- Собирает приложение для osx-x64 и osx-arm64
- Создает .app bundle с правильной структурой
- Конвертирует PNG иконку в .icns формат
- Упаковывает в DMG файл
- Создает GitHub Release с артефактами

### 2. `release.yml` - Полный релиз
**Назначение**: Создает полный релиз для всех платформ (Windows + macOS)

**Запуск**:
- Автоматически при создании тега версии (`v*`)
- Вручную через GitHub Actions UI

**Что делает**:
- Собирает Windows установщик (.exe)
- Собирает macOS DMG файлы (Intel + Apple Silicon)
- Создает единый GitHub Release со всеми артефактами

## Как использовать

### Автоматический релиз
1. Создайте новый тег версии:
   ```bash
   git tag v2.1.0
   git push origin v2.1.0
   ```

2. GitHub Actions автоматически:
   - Соберет все версии приложения
   - Создаст GitHub Release
   - Загрузит все артефакты

### Ручной запуск
1. Перейдите в раздел "Actions" в GitHub
2. Выберите нужный workflow
3. Нажмите "Run workflow"
4. Введите версию (например, `2.1.0`)
5. Нажмите "Run workflow"

## Структура артефактов

### Windows
- `QuickMind-Setup-v{version}.exe` - Полный установщик с Inno Setup

### macOS
- `QuickMind-osx-x64-v{version}.dmg` - Для Intel Macs
- `QuickMind-osx-arm64-v{version}.dmg` - Для Apple Silicon Macs (M1/M2/M3)

## Требования

### Для Windows сборки
- Windows runner (автоматически)
- .NET 9.0 SDK
- Inno Setup (предустановлен на GitHub runners)

### Для macOS сборки
- macOS runner (автоматически)
- .NET 9.0 SDK
- Xcode command line tools (предустановлены)

## Особенности

### Конвертация иконок
Workflow автоматически конвертирует `Assets/QuickMindLogo.png` в `.icns` формат для macOS приложения, создавая все необходимые размеры:
- 16x16, 32x32, 128x128, 256x256, 512x512, 1024x1024
- Обычные и @2x (Retina) версии

### Подпись и нотаризация
Для продакшн релизов рекомендуется добавить:
- Подпись кода для macOS (требует Apple Developer аккаунт)
- Нотаризацию для обхода Gatekeeper

### Структура .app bundle
Создается правильная структура macOS приложения:
```
QuickMind.app/
├── Contents/
│   ├── Info.plist
│   ├── MacOS/
│   │   └── QuickMind (исполняемый файл)
│   └── Resources/
│       └── QuickMind.icns (иконка)
```

## Отладка

### Логи
- Все логи сборки доступны в разделе "Actions"
- Каждый шаг подробно документирован

### Проблемы с DMG
Если DMG не создается:
1. Проверьте, что `hdiutil` доступен
2. Убедитесь, что .app bundle создан корректно
3. Проверьте размер временного образа

### Проблемы с иконкой
Если иконка не отображается:
1. Убедитесь, что файл `Assets/QuickMindLogo.png` существует
2. Проверьте, что `sips` и `iconutil` доступны
3. Убедитесь, что .icns файл скопирован в Resources

## Локальное тестирование

### Тестирование macOS сборки
```bash
# Сборка для конкретной архитектуры
dotnet publish -c Release -r osx-arm64 --self-contained true -o "./publish/osx-arm64"

# Создание .app bundle
./installer/Build-macOS-Installer.sh
```

### Тестирование Windows сборки
```powershell
# Сборка установщика
./installer/Build-Windows-Installer.ps1 -Version "2.1.0"
```

## Результат

После успешной сборки:
1. Релиз появится в разделе "Releases"
2. Все файлы будут автоматически загружены
3. Пользователи смогут скачать установщики для своих платформ

## Безопасность

- Все workflows используют официальные GitHub Actions
- Токены доступа автоматически управляются GitHub
- Артефакты хранятся в безопасном облаке GitHub 