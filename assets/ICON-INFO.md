# Иконка FlashCard App

## 📁 Расположение

Иконка приложения находится в файле: `assets/icon.ico`

**Размер файла**: 32,832 байт  
**Формат**: Windows Icon (.ico)  
**Рекомендуемые размеры**: 16x16, 32x32, 48x48, 256x256 пикселей

## ⚙️ Настройка в проекте

### 1. В FlashCardApp.csproj
```xml
<PropertyGroup>
  <ApplicationIcon>assets\icon.ico</ApplicationIcon>
  <Win32Resource />
</PropertyGroup>

<ItemGroup>
  <Content Include="assets\icon.ico">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 2. Что это дает:
- ✅ Иконка в исполняемом файле (.exe)
- ✅ Иконка в Проводнике Windows
- ✅ Иконка на панели задач
- ✅ Иконка в списке процессов

## 🛠️ Использование в инсталляторах

### PowerShell инсталлятор (`Install-FlashCardApp.ps1`)
```powershell
# Копирование иконки
$IconPath = Join-Path $PSScriptRoot "..\assets\icon.ico"
if (Test-Path $IconPath) {
    Copy-Item $IconPath $InstallPath -Force
}

# Создание ярлыка с иконкой
$Shortcut = $WshShell.CreateShortcut("$DesktopPath\$ShortcutName.lnk")
$Shortcut.TargetPath = "$InstallPath\FlashCardApp.exe"
if (Test-Path "$InstallPath\icon.ico") {
    $Shortcut.IconLocation = "$InstallPath\icon.ico"
}
$Shortcut.Save()
```

### Inno Setup инсталлятор (`FlashCardApp-Setup.iss`)
```ini
[Setup]
SetupIconFile=..\assets\icon.ico
UninstallDisplayIcon={app}\icon.ico

[Files]
Source: "..\assets\icon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\FlashCard App"; Filename: "{app}\FlashCardApp.exe"; IconFilename: "{app}\icon.ico"
Name: "{autodesktop}\FlashCard App"; Filename: "{app}\FlashCardApp.exe"; IconFilename: "{app}\icon.ico"
```

### Портативная версия (`Create-Portable.bat`)
```batch
copy "assets\icon.ico" "%PORTABLE_DIR%\" >nul 2>&1
```

## 📋 Автоматическое копирование

Все скрипты сборки автоматически копируют иконку:
- `Build-Installer.bat` - копирует в папку `publish`
- `Create-Portable.bat` - копирует в портативную версию
- `Install-FlashCardApp.ps1` - копирует при установке

## 🎨 Создание своей иконки

### Требования:
- **Формат**: .ico (Windows Icon)
- **Размеры**: Мультиразмерная (16x16, 32x32, 48x48, 64x64, 128x128, 256x256)
- **Цветность**: 32-bit с альфа-каналом (прозрачность)
- **Максимальный размер файла**: ~100 КБ

### Инструменты для создания:
1. **GIMP** (бесплатно) - File → Export As → .ico
2. **Paint.NET** + плагин ICO - бесплатно
3. **Adobe Photoshop** - с плагином ICO
4. **Онлайн конвертеры**:
   - https://convertio.co/png-ico/
   - https://icoconvert.com/
   - https://favicon.io/

### Рекомендации для дизайна:
- 🎯 **Простота** - иконка должна быть понятна в маленьком размере
- 🎨 **Контрастность** - хорошо видна на разных фонах
- 📐 **Масштабируемость** - читаема в размере 16x16
- 🔲 **Квадратная** - иконки Windows обычно квадратные
- 💡 **Тематичность** - отражает назначение приложения

### Тематика для FlashCard App:
- 📚 Книги или карточки
- 🧠 Мозг или обучение
- 📝 Заметки или текст
- 🎓 Образование
- 💡 Лампочка (идея/знания)

## 🔄 Замена иконки

Для замены текущей иконки:

1. **Подготовьте новую иконку**: `new-icon.ico`
2. **Замените файл**: `assets/icon.ico`
3. **Пересоберите проект**: `dotnet build -c Release`
4. **Опубликуйте**: `dotnet publish ...`
5. **Обновите инсталляторы**: запустите `Build-Installer.bat`

## ✅ Проверка иконки

После замены проверьте:
- [ ] Иконка отображается в `FlashCardApp.exe`
- [ ] Ярлыки используют новую иконку
- [ ] Инсталлятор показывает правильную иконку
- [ ] Портативная версия содержит иконку

## 🎯 Текущая иконка

Текущая иконка представляет концепцию флэш-карточек для обучения и отлично подходит для приложения изучения материалов.