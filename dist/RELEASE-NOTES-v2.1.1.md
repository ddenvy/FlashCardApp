# QuickMind v2.1.1 Release Notes

**Release Date:** December 19, 2024

## 🐛 Bug Fixes

### Language Selection Improvements
- **Fixed initial language display**: The language selection window now always displays in English on first launch, ensuring consistent user experience
- **Improved language initialization logic**: Removed dependency on system locale for initial language setup
- **Better language persistence**: Subsequent launches now properly respect the user's previously selected language
- **Fixed card button localization**: Edit and Delete buttons in flashcards now properly display in the selected language instead of always showing in English

## 📝 Technical Changes

- Refactored `LocalizationService` constructor to ensure English is always the default language on first run
- Removed system culture dependency that could cause inconsistent initial language display
- Improved language loading and saving logic for better reliability
- Updated card templates in `MainWindow.axaml` to use proper data binding for Edit/Delete button labels

## 🚀 Installation

### Windows
- **Installer**: `QuickMind-Setup-v2.1.1.exe` (32.56 MB)

### macOS  
- **Archive**: `QuickMind-macOS-ARM64-v2.1.0.zip` (44.83 MB)
- *Note: macOS version number will be updated in the next release*

## 🔄 Upgrade Notes

This is a minor patch release that improves the language selection experience. All existing data and settings will be preserved during the upgrade.

## 🏷️ Previous Versions

- **v2.1.0**: Initial dark theme release with comprehensive UI redesign
- **v1.x**: Legacy versions with original UI

---

**Download the latest version from the [main README](../README.md)** 