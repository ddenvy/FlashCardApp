# QuickMind v2.2.0 Release Notes

## 🎉 Major Release: Advanced Spaced Repetition & Anki Import

This is a significant update that transforms QuickMind from a basic flashcard app into a sophisticated spaced repetition learning system comparable to Anki.

### 🚀 New Features

#### 🎯 Advanced Spaced Repetition Algorithm (SM-2+)
- **Multi-step Learning Process**: New cards now progress through configurable learning steps
- **Relearning System**: Failed cards automatically enter relearning cycle
- **Leech Detection**: Automatically identifies and suspends problematic cards (8+ failures)
- **Interval Fuzzing**: Randomizes intervals by ±25% to prevent card clustering
- **Flexible Algorithm Settings**: Customize learning steps, graduating intervals, and more

#### 📊 Enhanced Rating System
- **4-Point Rating Scale**: Replaced 6-button system with intuitive Again/Hard/Good/Easy
- **Color-Coded Buttons**: Red (Again), Orange (Hard), Green (Good), Blue (Easy)
- **Adaptive Intervals**: Smart interval calculation based on card difficulty and performance

#### 📥 Comprehensive Import System
- **Anki JSON Import**: Import exported Anki decks seamlessly
- **CSV File Support**: Import from Excel/Google Sheets with customizable columns
- **Text File Import**: Support for various text formats with configurable separators
- **Batch Import**: Import hundreds of cards at once with progress tracking

#### 📈 Advanced Statistics & Analytics
- **Daily Dashboard**: Cards due today, studied, and completion rates
- **Topic-based Statistics**: Detailed progress tracking per subject
- **Learning History**: Comprehensive session statistics and trends
- **Performance Metrics**: Success rates, average intervals, and difficulty analysis

#### 🎨 Improved User Interface
- **Responsive Design**: Better layout adaptation for different screen sizes
- **Enhanced Study Mode**: Cleaner interface with improved card presentation
- **Import Dialog**: Dedicated interface for file imports with preview
- **Visual Feedback**: Better animations and state transitions

#### 🌍 Enhanced Localization
- **Complete Russian Translation**: All new features fully localized
- **Chinese Language Support**: Extended translations for new components
- **Dynamic Language Switching**: Instant interface updates when changing languages

### 🔧 Technical Improvements

#### 🏗️ Architecture Enhancements
- **Dependency Injection**: Proper DI container setup with Microsoft.Extensions.DependencyInjection
- **Service Layer**: Dedicated services for spaced repetition, import, and card management
- **Database Migrations**: Automatic schema updates with new fields for algorithm support

#### 📊 New Data Model
- **Extended Card Properties**: Added CardType, LearningStep, Lapses, IsLeech, IsSuspended
- **Algorithm Fields**: Interval, Repetitions, EaseFactor, DueDate, FuzzFactor
- **Learning Tracking**: LearningDueDate, GraduatingInterval, EasyInterval

#### 🛠️ New Services
- **SpacedRepetitionService**: Complete implementation of SM-2+ algorithm
- **AnkiImportService**: Handles various import formats and data validation
- **Enhanced CardService**: Extended with topic management and advanced statistics

### 🐛 Bug Fixes

- **Study Session Completion**: Fixed issue where study sessions wouldn't end properly
- **Card Movement**: Resolved kanban board not updating card positions correctly
- **UI Responsiveness**: Fixed rating buttons overlapping on smaller screens
- **Database Consistency**: Improved data integrity and migration handling

### 📱 User Experience Improvements

- **Intuitive Study Flow**: Streamlined learning process with clear visual feedback
- **Better Card Management**: Enhanced drag-and-drop functionality between columns
- **Import Workflow**: User-friendly import process with error handling and validation
- **Performance Optimization**: Faster card loading and smoother animations

### 🔄 Migration Notes

- **Database Auto-Migration**: Existing cards automatically upgraded to new schema
- **Settings Preservation**: All user preferences maintained during update
- **Backward Compatibility**: Existing study data preserved and enhanced

### 📦 Installation

- **Windows**: `QuickMind-Setup-v2.2.0.exe` (34.2 MB)
- **Automatic Updates**: Existing installations will be notified of available update
- **Clean Installation**: Fresh installs include all new features by default

### 🎓 Learning Algorithm Details

The new SM-2+ algorithm includes:
- **Initial Learning**: 1m, 10m intervals for new cards
- **Graduating Interval**: 1 day for cards completing learning
- **Easy Interval**: 4 days for cards rated as "Easy"
- **Maximum Interval**: 36,500 days (100 years) limit
- **Leech Threshold**: 8 failures before card suspension

### 🔮 Coming Soon

- **APKG Import**: Direct Anki package file support
- **Advanced Statistics**: Detailed learning analytics and progress charts
- **Custom Themes**: Additional color schemes and customization options
- **Sync Functionality**: Cloud synchronization across devices

---

**Full Changelog**: [v2.1.2...v2.2.0](https://github.com/ddenvy/QuickMind/compare/v2.1.2...v2.2.0)

**Download**: [QuickMind-Setup-v2.2.0.exe](https://github.com/ddenvy/QuickMind/releases/download/v2.2.0/QuickMind-Setup-v2.2.0.exe)

---

*Made with ❤️ for learners everywhere!* 