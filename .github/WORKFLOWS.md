# GitHub Actions Workflows for QuickMind

This repository contains automated workflows for building and releasing the QuickMind application across different platforms.

## Workflows

### 1. `build-macos.yml` - macOS Build
**Purpose**: Creates DMG files for macOS (Intel and Apple Silicon)

**Triggers**:
- Automatically on version tag creation (`v*`)
- Manually via GitHub Actions UI
- On pull requests to main branch

**What it does**:
- Builds application for osx-x64 and osx-arm64
- Creates .app bundle with proper structure
- Converts PNG icon to .icns format
- Packages into DMG file
- Creates GitHub Release with artifacts

### 2. `release.yml` - Full Release
**Purpose**: Creates complete release for all platforms (Windows + macOS)

**Triggers**:
- Automatically on version tag creation (`v*`)
- Manually via GitHub Actions UI

**What it does**:
- Builds Windows installer (.exe)
- Builds macOS DMG files (Intel + Apple Silicon)
- Creates unified GitHub Release with all artifacts

## How to Use

### Automatic Release
1. Create a new version tag:
   ```bash
   git tag v2.1.1
   git push origin v2.1.1
   ```

2. GitHub Actions will automatically:
   - Build all application versions
   - Create GitHub Release
   - Upload all artifacts

### Manual Execution
1. Go to "Actions" section in GitHub
2. Select desired workflow
3. Click "Run workflow"
4. Enter version (e.g., `2.1.1`)
5. Click "Run workflow"

## Artifact Structure

### Windows
- `QuickMind-Setup-v{version}.exe` - Full installer with Inno Setup

### macOS
- `QuickMind-osx-x64-v{version}.dmg` - For Intel Macs
- `QuickMind-osx-arm64-v{version}.dmg` - For Apple Silicon Macs (M1/M2/M3)

## Requirements

### For Windows Build
- Windows runner (automatic)
- .NET 9.0 SDK
- Inno Setup (pre-installed on GitHub runners)

### For macOS Build
- macOS runner (automatic)
- .NET 9.0 SDK
- Xcode command line tools (pre-installed)

## Features

### Icon Conversion
Workflow automatically converts `Assets/QuickMindLogo.png` to `.icns` format for macOS application, creating all necessary sizes:
- 16x16, 32x32, 128x128, 256x256, 512x512, 1024x1024
- Regular and @2x (Retina) versions

### Code Signing and Notarization
For production releases, consider adding:
- Code signing for macOS (requires Apple Developer account)
- Notarization to bypass Gatekeeper

### .app Bundle Structure
Creates proper macOS application structure:
```
QuickMind.app/
├── Contents/
│   ├── Info.plist
│   ├── MacOS/
│   │   └── QuickMind (executable)
│   └── Resources/
│       └── QuickMind.icns (icon)
```

## Debugging

### Logs
- All build logs available in "Actions" section
- Each step is thoroughly documented

### DMG Issues
If DMG is not created:
1. Check that `hdiutil` is available
2. Ensure .app bundle is created correctly
3. Verify temporary image size

### Icon Issues
If icon doesn't display:
1. Ensure `Assets/QuickMindLogo.png` file exists
2. Check that `sips` and `iconutil` are available
3. Verify .icns file is copied to Resources

## Local Testing

### Testing macOS Build
```bash
# Build for specific architecture
dotnet publish -c Release -r osx-arm64 --self-contained true -o "./publish/osx-arm64"

# Create .app bundle
./installer/Build-macOS-Installer.sh
```

### Testing Windows Build
```powershell
# Build installer
./installer/Build-Windows-Installer.ps1 -Version "2.1.1"
```

## Result

After successful build:
1. Release appears in "Releases" section
2. All files are automatically uploaded
3. Users can download installers for their platforms

## Security

- All workflows use official GitHub Actions
- Access tokens are automatically managed by GitHub
- Artifacts are stored securely in GitHub cloud 