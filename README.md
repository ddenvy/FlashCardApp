# QuickMind 🧠

<div align="center">
  <img src="Assets/QuickMindLogo.png" alt="QuickMind Logo" width="120" height="120">
  
  **A modern, minimalist flashcard application for efficient learning**
  
  [![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
  [![Avalonia UI](https://img.shields.io/badge/Avalonia%20UI-11.0-blue.svg)](https://avaloniaui.net/)
  [![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
</div>

## 📥 Download

Ready to start learning? Download QuickMind for your platform:

<div align="center">
  
  [![Windows Download](https://img.shields.io/badge/Windows-Download-0078D4?style=for-the-badge&logo=windows&logoColor=white)](https://github.com/ddenvy/QuickMind/releases/latest)
  [![macOS Download](https://img.shields.io/badge/macOS-Download-000000?style=for-the-badge&logo=apple&logoColor=white)](https://github.com/ddenvy/QuickMind/releases/latest)
  [![Linux Download](https://img.shields.io/badge/Linux-Download-FCC624?style=for-the-badge&logo=linux&logoColor=black)](https://github.com/ddenvy/QuickMind/releases/latest)
  
  **Windows**: `QuickMind-Setup-v2.1.2.exe` (34.15 MB)  
  **macOS**: DMG files for Intel & Apple Silicon (45 MB)
  **Linux**: Multiple package formats available (35-50 MB)
  
  *Latest version: v2.1.2*
  
</div>

### Installation Instructions

- **Windows**: Download and run the `.exe` installer from [Releases](https://github.com/ddenvy/QuickMind/releases)
- **macOS**: Download the appropriate `.dmg` file for your Mac (Intel or Apple Silicon)
- **Linux**: Choose from multiple package formats:
  - **DEB** (Ubuntu/Debian): `quickmind_VERSION_amd64.deb`
  - **RPM** (Fedora/RHEL/openSUSE): `quickmind-VERSION-1.*.x86_64.rpm`
  - **AppImage** (Universal): `QuickMind-VERSION-x86_64.AppImage`
  - **Flatpak**: `QuickMind-VERSION.flatpak`
  - **Snap**: `quickmind_VERSION_amd64.snap`
  - **Source packages** for Arch Linux and Gentoo

📖 **Detailed Linux installation guide**: [docs/LINUX-INSTALL.md](docs/LINUX-INSTALL.md)

---

## ✨ Features

- **📚 Smart Flashcard System** - Organize your learning with three card states: New, Learning, and Known
- **🎯 Study Mode** - Interactive learning experience with progress tracking
- **🌍 Multi-language Support** - English, Russian, and Chinese interface
- **🎨 Dark Minimalist Design** - Clean, distraction-free interface inspired by modern learning platforms
- **📝 Easy Card Management** - Add, edit, and delete flashcards with live preview
- **🏷️ Topic Organization** - Group cards by subjects or topics
- **⚡ Cross-platform** - Available for Windows, macOS, and Linux (DEB, RPM, AppImage, Flatpak, Snap)

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Git (for cloning the repository)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/QuickMind.git
   cd QuickMind
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

### Building for Release

#### Local Build
To create a release build locally:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Replace `win-x64` with your target platform:
- Windows: `win-x64`, `win-arm64`
- macOS: `osx-x64`, `osx-arm64`
- Linux: `linux-x64`, `linux-arm64`

#### Automated Installers

QuickMind includes GitHub Actions workflows for automatic installer creation:

**Windows Installer (Inno Setup)**
```bash
./installer/Build-Windows-Installer.ps1 -Version "2.1.0"
```

**macOS DMG (Intel + Apple Silicon)**
```bash
./installer/Build-macOS-Installer.sh
```

**Linux Packages (via GitHub Actions)**
- DEB packages for Debian/Ubuntu
- RPM packages for Red Hat/Fedora/openSUSE  
- AppImage for universal compatibility
- Flatpak and Snap packages
- Source packages for Arch Linux and Gentoo

**All Platforms**
```bash
./installer/Build-All.ps1 -Version "2.1.0"
```

#### GitHub Actions

For automatic releases, simply create and push a version tag:

```bash
git tag v2.1.0
git push origin v2.1.0
```

This will trigger GitHub Actions to:
- Build Windows installer (.exe)
- Build macOS DMG files (Intel + Apple Silicon)
- Build Linux packages (DEB, RPM, AppImage, Flatpak, Snap)
- Create source packages for Arch Linux and Gentoo
- Create a GitHub Release with all files
- Generate release notes automatically

See [.github/WORKFLOWS.md](.github/WORKFLOWS.md) for detailed workflow documentation.

## 📖 How to Use

### First Launch
1. Select your preferred language from the welcome screen
2. Start adding your first flashcards using the "Add Card" button

### Creating Flashcards
1. Click **"Add Card"** in the main window
2. Fill in the question and answer fields
3. Choose an existing topic or create a new one
4. Preview your card and save

### Studying
1. Click **"Study Mode"** to begin learning
2. Read the question and try to recall the answer
3. Click **"Show Answer"** to reveal the correct answer
4. Rate your knowledge:
   - **"Don't Know"** - moves card back to learning pile
   - **"Know"** - moves card to known pile

### Card Management
- **New Cards**: Recently added cards waiting to be studied
- **Learning Cards**: Cards you're actively studying
- **Known Cards**: Cards you've mastered

## 🛠️ Technology Stack

- **Framework**: .NET 9.0
- **UI Framework**: Avalonia UI 11.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **Database**: SQLite with Entity Framework Core
- **Styling**: Custom dark theme with Corus font

## 🎨 Design Philosophy

QuickMind follows a minimalist design approach inspired by modern learning platforms like YouLearn:

- **Dark Theme**: Reduces eye strain during long study sessions
- **Clean Typography**: Corus font for optimal readability
- **Subtle Colors**: Muted color palette for distraction-free learning
- **Intuitive Layout**: Three-column card organization for easy progress tracking

## 📁 Project Structure

```
QuickMind/
├── Assets/                 # Images and icons
├── Converters/            # Value converters for data binding
├── Models/                # Data models and database context
├── Services/              # Business logic and services
├── ViewModels/            # MVVM view models
├── Views/                 # UI views and windows
├── App.axaml             # Application styles and resources
├── Program.cs            # Application entry point
└── QuickMind.csproj      # Project configuration
```

## 🌍 Supported Languages

- 🇺🇸 **English** - Full support
- 🇷🇺 **Russian** - Full support  
- 🇨🇳 **Chinese** - Full support

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow MVVM pattern for UI logic
- Use meaningful commit messages
- Add comments for complex logic
- Test your changes across different platforms

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- [Avalonia UI](https://avaloniaui.net/) for the excellent cross-platform UI framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) for database management
- The open-source community for inspiration and tools

## 📞 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/your-username/QuickMind/issues) page
2. Create a new issue with detailed information
3. Include your OS version and steps to reproduce

---

<div align="center">
  Made with ❤️ for learners everywhere !
  
  **Happy Learning! 🎓**
</div> 