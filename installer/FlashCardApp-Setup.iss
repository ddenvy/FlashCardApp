[Setup]
; Основные настройки приложения
AppId={{B4C8F4A2-4F2E-4B3D-8A7C-1E9F5D3A2C4B}
AppName=FlashCard App
AppVersion=1.0.0
AppVerName=FlashCard App 1.0.0
AppPublisher=FlashCard App Team
AppPublisherURL=https://github.com/ddenvy/FlashCardApp
AppSupportURL=https://github.com/ddenvy/FlashCardApp/issues
AppUpdatesURL=https://github.com/ddenvy/FlashCardApp/releases
DefaultDirName={autopf}\FlashCardApp
DefaultGroupName=FlashCard App
AllowNoIcons=yes
LicenseFile=..\LICENSE.txt
InfoBeforeFile=..\README.md
OutputDir=..\dist
OutputBaseFilename=FlashCardApp-Setup-v1.0.0
SetupIconFile=..\assets\icon.ico
UninstallDisplayIcon={app}\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

; Языки
[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

; Задачи
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1

; Файлы для установки
[Files]
Source: "..\publish\FlashCardApp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publish\FlashCardApp.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsDebugMode
Source: "..\assets\icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

; Ярлыки
[Icons]
Name: "{group}\FlashCard App"; Filename: "{app}\FlashCardApp.exe"; IconFilename: "{app}\icon.ico"
Name: "{group}\Документация"; Filename: "{app}\README.md"
Name: "{group}\{cm:UninstallProgram,FlashCard App}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\FlashCard App"; Filename: "{app}\FlashCardApp.exe"; IconFilename: "{app}\icon.ico"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\FlashCard App"; Filename: "{app}\FlashCardApp.exe"; IconFilename: "{app}\icon.ico"; Tasks: quicklaunchicon

; Запуск после установки
[Run]
Filename: "{app}\FlashCardApp.exe"; Description: "{cm:LaunchProgram,FlashCard App}"; Flags: nowait postinstall skipifsilent

; Регистрация в системе
[Registry]
Root: HKLM; Subkey: "SOFTWARE\FlashCardApp"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"
Root: HKLM; Subkey: "SOFTWARE\FlashCardApp"; ValueType: string; ValueName: "Version"; ValueData: "1.0.0"

; Код для проверки режима отладки
[Code]
function IsDebugMode: Boolean;
begin
  Result := FileExists(ExpandConstant('{src}\..\publish\FlashCardApp.pdb'));
end;

// Пользовательские сообщения
procedure InitializeWizard;
begin
  WizardForm.WelcomeLabel1.Caption := 'Добро пожаловать в мастер установки FlashCard App';
  WizardForm.WelcomeLabel2.Caption := 'Приложение для изучения с флэш-карточками' + #13#13 +
    'Это приложение поможет вам эффективно изучать различные темы ' +
    'с помощью интерактивных карточек и канбан-доски.' + #13#13 +
    'Нажмите "Далее" для продолжения или "Отмена" для выхода из программы установки.';
end;

// Проверка версии .NET
function IsDotNetInstalled: Boolean;
var
  Success: Boolean;
  InstallSuccess: Boolean;
  ResultCode: Integer;
begin
  Result := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full') or
            RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\Microsoft\NET Framework Setup\NDP\v4\Full') or
            FileExists(ExpandConstant('{pf}\dotnet\dotnet.exe')) or
            FileExists(ExpandConstant('{pf32}\dotnet\dotnet.exe'));
end;

function InitializeSetup: Boolean;
begin
  Result := True;
  if not IsDotNetInstalled then
  begin
    if MsgBox('Для работы FlashCard App требуется .NET Framework 4.8 или .NET 8.' + #13#10 +
              'Хотите продолжить установку?', mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
    end;
  end;
end;

// Создание дополнительных ярлыков
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Создание дополнительного ярлыка в меню "Пуск"
    CreateShellLink(
      ExpandConstant('{autoprograms}\FlashCard App.lnk'),
      ExpandConstant('{app}\FlashCardApp.exe'),
      '',
      ExpandConstant('{app}'),
      'Приложение для изучения с флэш-карточками',
      '',
      0,
      SW_SHOWNORMAL
    );
  end;
end; 