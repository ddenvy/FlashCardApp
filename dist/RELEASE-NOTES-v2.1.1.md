# QuickMind v2.1.1 Fixes and Improvements

## What's Fixed

### 🔧 Taskbar Icon Fix
- **Embedded Icon**: QuickMind icon now properly displays in Windows taskbar
- **Project Configuration**: Added `ApplicationIcon` parameter to project file
- **Consistency**: Icon is now uniform everywhere - in application window, taskbar, and shortcuts

### 🌐 Localization Fix
- **Card Buttons**: Fixed display of "Edit" and "Delete" buttons - they now correctly translate to the selected language
- **Default Language**: On first launch, the application always starts in English, regardless of system settings
- **Choice Saving**: Selected language is saved and applied on subsequent launches

### 🎨 Shortcut Improvements
- **Simplification**: Removed unnecessary icon settings from installer for better compatibility
- **Clean Code**: Removed all comments from source code - now the code is self-documenting

## Technical Details

- **Version**: 2.1.1
- **Installer Size**: 34.15 MB
- **Compatibility**: Windows 10/11 (64-bit)
- **Dependencies**: .NET 9.0 (included in installer)

## Download

- **Windows**: [QuickMind-Setup-v2.1.1.exe](./QuickMind-Setup-v2.1.1.exe) (34.15 MB)
- **macOS**: [QuickMind-macOS-ARM64-v2.1.0.zip](./QuickMind-macOS-ARM64-v2.1.0.zip) (45 MB)

## Features

### Flashcard Learning
- Add cards with questions and answers
- Study in interactive mode
- Progress system: New → Learning → Known

### Multi-language Support
- Supported languages: English, Russian, 中文
- Instant language switching
- Localization of all interface elements

### Modern Design
- Dark theme for comfortable learning
- Corus font for better readability
- Minimalist and intuitive interface

**Previous version**: [v2.1.0](./RELEASE-NOTES-v2.1.0.md) 