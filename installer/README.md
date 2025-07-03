# QuickMind Installer Builder

This directory contains scripts for creating QuickMind installers for Windows and macOS.

## 📋 Prerequisites

### General Requirements
- **.NET 9.0 SDK** or newer
- **Git** (for cloning the repository)

### For Windows
- **Inno Setup 6.0+** - download from [official website](https://jrsoftware.org/isdownload.php)
- **PowerShell 5.1+** (usually already installed)

### For macOS
- **Xcode Command Line Tools**: `xcode-select --install`
- **Bash** (usually already installed)
- Optional: **create-dmg** for nicer DMG files

## 🚀 Quick Start

### Automatic Build for All Installers
```powershell
# Build all installers
.\installer\Build-All.ps1

# Build for Windows only
.\installer\Build-All.ps1 -Platform Windows

# Build for macOS only
.\installer\Build-All.ps1 -Platform macOS

# Build with specific version
.\installer\Build-All.ps1 -Version "2.1.0"
```

### Manual Build

#### Windows
```powershell
.\installer\Build-Windows-Installer.ps1
```

#### macOS
```bash
./installer/Build-macOS-Installer.sh
```

## 📁 File Structure

```
installer/
├── Build-All.ps1              # Universal build script
├── Build-Windows-Installer.ps1 # Windows script
├── Build-macOS-Installer.sh   # macOS script
├── QuickMind-Setup.iss        # Inno Setup configuration (generated)
└── README.md                  # This file

dist/                          # Ready installers
├── QuickMind-Setup-v2.0.0.exe # Windows installer
├── QuickMind-v2.0.0.dmg       # macOS DMG
└── QuickMind.app/             # macOS App Bundle

publish/                       # Compiled files
├── win-x64/                   # Windows build
└── osx-x64/                   # macOS build
```

## 🔧 Configuration

### Changing Version
Edit the `QuickMind.csproj` file:
```xml
<Version>2.0.0</Version>
<FileVersion>2.0.0</FileVersion>
```

### Icon Setup
- **Windows**: Place `.ico` file in `Assets/` and update the path in Inno Setup script
- **macOS**: Create `.icns` file and place in `Contents/Resources/` app bundle

## 📦 Generated Files

### Windows
- **QuickMind-Setup-v{version}.exe** - Windows installer
  - Installs application to `Program Files`
  - Creates shortcuts in Start menu and desktop
  - Supports automatic uninstall

### macOS
- **QuickMind.app** - macOS application
- **QuickMind-v{version}.dmg** - DMG image for installation
  - Contains app bundle and link to Applications folder
  - Ready for distribution

## 🔐 Code Signing and Notarization

### Windows (optional)
```powershell
# Signing (requires certificate)
signtool sign /f certificate.pfx /p password QuickMind-Setup-v2.0.0.exe
```

### macOS (recommended for distribution)
```bash
# Sign app bundle
codesign --force --deep --sign "Developer ID Application: Your Name" QuickMind.app

# Notarization (requires Apple Developer account)
xcrun notarytool submit QuickMind-v2.0.0.dmg --keychain-profile "notarytool-profile" --wait

# Staple notarization
xcrun stapler staple QuickMind-v2.0.0.dmg
```

## 🐛 Troubleshooting

### Windows
- **Error "Inno Setup not found"**: Install Inno Setup and add it to PATH
- **Build error**: Check that .NET SDK is properly installed
- **Code signing issues**: Ensure certificate is valid

### macOS
- **Error "hdiutil not found"**: Command should be available on all macOS systems
- **Icon issues**: Use `iconutil` to create .icns file
- **Gatekeeper blocks**: Sign and notarize the application

## 📝 Notes

- Installers create self-contained builds (don't require .NET installation)
- Installer size: ~60-100 MB (includes .NET Runtime)
- Supported versions: Windows 10+ (x64), macOS 10.15+ (x64)

## 🔗 Useful Links

- [Inno Setup Documentation](https://jrsoftware.org/isinfo.php)
- [.NET Publishing Guide](https://docs.microsoft.com/en-us/dotnet/core/deploying/)
- [macOS App Distribution](https://developer.apple.com/documentation/xcode/distributing_your_app_for_beta_testing_and_releases)
- [Code Signing Guide](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution) 