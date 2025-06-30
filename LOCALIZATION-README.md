# Система локализации FlashCard App

## Обзор

В приложение FlashCard App была добавлена полная поддержка многоязычности с возможностью переключения между русским, английским и китайским (упрощенным) языками в реальном времени.

## Поддерживаемые языки

- **Русский (ru)** - язык по умолчанию
- **English (en)** - английский язык  
- **中文简体 (zh-CN)** - упрощенный китайский

## Архитектура локализации

### 1. Ресурсные файлы

Создано три ресурсных файла в папке `Resources/`:

- `Strings.resx` - основной файл с русскими строками
- `Strings.en.resx` - английские переводы
- `Strings.zh-CN.resx` - китайские переводы

### 2. LocalizationService

Создан синглтон-сервис `LocalizationService` который:

- Управляет текущей культурой приложения
- Предоставляет список доступных языков
- Сохраняет выбранный язык в настройках пользователя
- Уведомляет об изменении языка через событие `LanguageChanged`

### 3. Конвертер локализации

`LocalizationConverter` - WPF конвертер для биндинга в XAML:

```xml
<TextBlock Text="{Binding CurrentCulture, Source={x:Static services:LocalizationService.Instance}, 
                  Converter={StaticResource LocalizationConverter}, 
                  ConverterParameter=MainWindow_Title}"/>
```

### 4. Расширение разметки

`LocalizeExtension` - упрощенный синтаксис для XAML:

```xml
<TextBlock Text="{services:Localize MainWindow_Title}"/>
```

## Использование в XAML

### Обычный биндинг
```xml
Text="{Binding CurrentCulture, Source={x:Static services:LocalizationService.Instance}, 
       Converter={StaticResource LocalizationConverter}, 
       ConverterParameter=KeyName}"
```

### Расширение разметки (упрощенный синтаксис)
```xml
Text="{services:Localize KeyName}"
```

## Переключение языков

В главном окне добавлен выпадающий список для выбора языка:

```xml
<ComboBox ItemsSource="{Binding AvailableLanguages, Source={x:Static services:LocalizationService.Instance}}" 
          SelectedValue="{Binding CurrentLanguage}"
          SelectedValuePath="Code"
          DisplayMemberPath="NativeName"/>
```

При выборе нового языка:
1. Обновляется культура приложения
2. Сохраняется настройка в `Properties.Settings.Default.Language`
3. Все UI элементы автоматически обновляются

## ViewModels

MainWindowViewModel подписывается на событие изменения языка и обновляет:
- Строку "Все темы" в соответствии с выбранным языком
- Текущий язык в биндингах

## Настройки

Выбранный язык сохраняется в `Properties/Settings.settings`:

```xml
<Setting Name="Language" Type="System.String" Scope="User">
  <Value Profile="(Default)">ru</Value>
</Setting>
```

## Автоматическое обновление UI

Благодаря биндингу на `CurrentCulture` и событию `LanguageChanged`, все элементы интерфейса автоматически обновляются при смене языка без перезапуска приложения.

## Добавление новых строк

1. Добавить строку во все три ресурсных файла
2. Использовать в XAML через LocalizationConverter или LocalizeExtension
3. При необходимости получить в коде: `LocalizationService.Instance.GetString("KeyName")`

## Добавление нового языка

1. Создать новый файл `Strings.{culture}.resx`
2. Добавить культуру в `LocalizationService.AvailableLanguages`
3. Перевести все строки

Система автоматически подхватит новый язык без изменения кода. 