# 📁 QuickMind Project Structure

## 🏗️ Architecture
QuickMind is built on **.NET 9.0** using **Avalonia UI** following the **MVVM** pattern.

## 📂 Folder Structure

### 🔧 Core Files
```
QuickMind/
├── App.axaml                  # Application styles and resources
├── App.axaml.cs              # Application initialization logic
├── Program.cs                # Application entry point
├── QuickMind.csproj          # Project configuration
├── ViewLocator.cs            # View and ViewModel binding
├── app.manifest              # Windows manifest
└── .gitignore                # Git ignored files
```

### 🎯 Models (Models/)
```
Models/
├── FlashCard.cs              # FlashCard model
└── FlashCardContext.cs       # EF Core database context
```

### 🧠 ViewModels (ViewModels/)
```
ViewModels/
├── MainWindowViewModel.cs           # Main window
├── StudyModeViewModel.cs           # Study mode
├── AddCardDialogViewModel.cs       # Add card dialog
├── LanguageSelectionViewModel.cs   # Language selection
├── ViewModelBase.cs               # Base ViewModel class
└── RelayCommand.cs                # MVVM commands
```

### 🖥️ Views (Views/)
```
Views/
├── MainWindow.axaml(.cs)           # Main window
├── StudyModeWindow.axaml(.cs)      # Study mode window
├── AddCardDialog.axaml(.cs)        # Add card dialog
└── LanguageSelectionWindow.axaml(.cs) # Language selection window
```

### ⚙️ Services (Services/)
```
Services/
├── CardService.cs            # Card operations
└── LocalizationService.cs   # Multi-language support
```

### 🔄 Converters (Converters/)
```
Converters/
└── LocalizationConverter.cs # Localization converter
```

### 🎨 Resources (Assets/)
```
Assets/
├── QuickMindLogo.png        # Application logo
└── avalonia-logo.ico        # Avalonia icon
```

### 🏗️ Build (installer/)
```
installer/
├── Build-All.ps1                    # Build for all platforms
├── Build-Windows-Installer.ps1     # Build Windows installer
├── Build-macOS-Installer.sh        # Build macOS DMG
├── QuickMind-Setup.iss             # Inno Setup configuration
└── README.md                       # Build documentation
```

### 📦 Distribution (dist/)
```
dist/
├── QuickMind-Setup-v2.1.0.exe          # Windows installer
├── QuickMind-macOS-ARM64-v2.1.0.zip    # macOS archive
├── RELEASE-NOTES-v2.1.0.md             # Release notes
├── README.md                           # User instructions
├── README-macOS.md                     # macOS-specific instructions
└── Install-QuickMind-macOS.sh          # macOS installation script
```

### 🤖 GitHub Actions (.github/)
```
.github/
├── workflows/
│   ├── build-macos.yml      # macOS DMG build
│   └── release.yml          # Full release for all platforms
└── README.md               # CI/CD documentation
```

## 🎨 Design and Styles

### Color Scheme
- **Background**: #1E1E1E (dark)
- **Panels**: #2A2A2A (dark gray)
- **Borders**: #333333 (muted gray)
- **Text**: #FFFFFF (white) / #B0B0B0 (light gray)

### Font
- **Primary**: Corus (used throughout the application)

### Components
- **Buttons**: Consistent sizing (MinHeight: 36px, Padding: 16,10)
- **Input Fields**: Consistent sizing (MinHeight: 36px, Padding: 8px)
- **Headers**: Equal width in kanban layout

## 🗄️ Database
- **Type**: SQLite with Entity Framework Core
- **Models**: FlashCard (Question, Answer, Topic, Status, CreatedAt, LastReviewed)
- **Card States**: New, Learning, Known

## 🌍 Multi-language Support
- **Supported Languages**: English, Russian, Chinese
- **System**: Custom LocalizationService with dynamic language switching

## 🚀 Build and Deployment

### Local Development
```bash
dotnet restore
dotnet run
```

### Creating Release
```bash
# All platforms
./installer/Build-All.ps1 -Version "2.1.0"

# Windows only
./installer/Build-Windows-Installer.ps1 -Version "2.1.0"

# macOS only (requires macOS)
./installer/Build-macOS-Installer.sh
```

### GitHub Actions
Automatic build when creating a tag:
```bash
git tag v2.1.0
git push origin v2.1.0
```

## 🔧 Development Requirements
- **.NET 9.0 SDK**
- **Visual Studio 2022** or **JetBrains Rider**
- **Git** for version control

## 📝 Configuration Files
- **QuickMind.csproj**: NuGet packages and project settings
- **.gitignore**: Git exclusions (bin/, obj/, publish/, dist/*.app/)
- **app.manifest**: Windows manifest (DPI awareness)

---

💡 **Tip**: For quick start, see [README.md](README.md) in the project root. 