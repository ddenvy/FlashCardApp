[Setup]
AppName=QuickMind
AppVersion=2.2.2
AppVerName=QuickMind v2.2.2
AppPublisher=QuickMind
AppPublisherURL=https://github.com/ddenvy/QuickMind
AppSupportURL=https://github.com/ddenvy/QuickMind/issues
AppUpdatesURL=https://github.com/ddenvy/QuickMind/releases
DefaultDirName={autopf}\QuickMind
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.txt
OutputDir=..\dist
OutputBaseFilename=QuickMind-Setup-v2.2.2
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
