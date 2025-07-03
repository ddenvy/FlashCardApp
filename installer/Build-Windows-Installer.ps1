# Build Windows Installer for QuickMind
param(
    [string]$Version = "2.1.1",
    [string]$Configuration = "Release"
)

Write-Host "Building QuickMind Windows Installer v$Version" -ForegroundColor Green

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path ".\publish") { Remove-Item ".\publish" -Recurse -Force }
if (Test-Path ".\dist\QuickMind-Setup-v$Version.exe") { Remove-Item ".\dist\QuickMind-Setup-v$Version.exe" -Force }

# Build and publish for Windows x64
Write-Host "Publishing application for Windows x64..." -ForegroundColor Yellow
dotnet publish ..\QuickMind.csproj -c $Configuration -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "..\publish\win-x64"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Check if Inno Setup is installed
$InnoSetupPath = Get-Command "iscc.exe" -ErrorAction SilentlyContinue
if (-not $InnoSetupPath) {
    $InnoSetupPath = Get-ChildItem -Path "${env:ProgramFiles(x86)}\Inno Setup*" -Filter "iscc.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $InnoSetupPath) {
        Write-Host "Inno Setup not found! Please install Inno Setup from https://jrsoftware.org/isdownload.php" -ForegroundColor Red
        Write-Host "Alternatively, manually create installer from files in .\publish\win-x64\" -ForegroundColor Yellow
        exit 1
    }
    $InnoSetupPath = $InnoSetupPath.FullName
} else {
    $InnoSetupPath = $InnoSetupPath.Source
}

Write-Host "Found Inno Setup at: $InnoSetupPath" -ForegroundColor Green

# Create Inno Setup script
Write-Host "Creating Inno Setup script..." -ForegroundColor Yellow
$InnoScript = @"
[Setup]
AppName=QuickMind
AppVersion=$Version
AppVerName=QuickMind v$Version
AppPublisher=QuickMind
AppPublisherURL=https://github.com/ddenvy/QuickMind
AppSupportURL=https://github.com/ddenvy/QuickMind/issues
AppUpdatesURL=https://github.com/ddenvy/QuickMind/releases
DefaultDirName={autopf}\QuickMind
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.txt
OutputDir=..\dist
OutputBaseFilename=QuickMind-Setup-v$Version
SetupIconFile=..\Assets\avalonia-logo.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\QuickMind"; Filename: "{app}\QuickMind.exe"
Name: "{autodesktop}\QuickMind"; Filename: "{app}\QuickMind.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\QuickMind.exe"; Description: "{cm:LaunchProgram,QuickMind}"; Flags: nowait postinstall skipifsilent
"@

$InnoScript | Out-File -FilePath ".\QuickMind-Setup.iss" -Encoding UTF8

# Build installer
Write-Host "Building installer..." -ForegroundColor Yellow
& $InnoSetupPath ".\QuickMind-Setup.iss"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Windows installer created successfully: .\dist\QuickMind-Setup-v$Version.exe" -ForegroundColor Green
    
    # Display file info
    $installerFile = Get-Item "..\dist\QuickMind-Setup-v$Version.exe"
    Write-Host "File size: $([math]::Round($installerFile.Length / 1MB, 2)) MB" -ForegroundColor Cyan
} else {
    Write-Host "❌ Installer build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build completed!" -ForegroundColor Green