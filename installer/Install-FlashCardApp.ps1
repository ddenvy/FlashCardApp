# FlashCard App Installer
# Установщик приложения FlashCard App

param(
    [string]$InstallPath = "$env:ProgramFiles\FlashCardApp",
    [switch]$Silent = $false
)

# Проверка прав администратора
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "Этот скрипт требует права администратора. Перезапуск..." -ForegroundColor Red
    Start-Process PowerShell -Verb RunAs "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" $($MyInvocation.UnboundArguments)"
    Exit
}

Write-Host "=== FlashCard App Installer ===" -ForegroundColor Green
Write-Host ""

# Определение путей
$SourcePath = Join-Path $PSScriptRoot "..\publish\FlashCardApp.exe"
$ShortcutName = "FlashCard App"
$DesktopPath = [Environment]::GetFolderPath("CommonDesktopDirectory")
$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"

if (-not $Silent) {
    Write-Host "Установка FlashCard App в: $InstallPath" -ForegroundColor Yellow
    $response = Read-Host "Продолжить? (Y/N)"
    if ($response -ne "Y" -and $response -ne "y") {
        Write-Host "Установка отменена." -ForegroundColor Red
        Exit
    }
}

try {
    # Создание каталога для установки
    Write-Host "Создание каталога: $InstallPath" -ForegroundColor Cyan
    if (-not (Test-Path $InstallPath)) {
        New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
    }

    # Копирование файлов
    Write-Host "Копирование файлов..." -ForegroundColor Cyan
    Copy-Item $SourcePath $InstallPath -Force
    
    if (Test-Path (Join-Path $PSScriptRoot "..\publish\FlashCardApp.pdb")) {
        Copy-Item (Join-Path $PSScriptRoot "..\publish\FlashCardApp.pdb") $InstallPath -Force
    }
    
    # Копирование иконки
    $IconPath = Join-Path $PSScriptRoot "..\assets\icon.ico"
    if (Test-Path $IconPath) {
        Copy-Item $IconPath $InstallPath -Force
    }

    # Создание ярлыка на рабочем столе
    Write-Host "Создание ярлыка на рабочем столе..." -ForegroundColor Cyan
    $WshShell = New-Object -comObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut("$DesktopPath\$ShortcutName.lnk")
    $Shortcut.TargetPath = "$InstallPath\FlashCardApp.exe"
    $Shortcut.WorkingDirectory = $InstallPath
    $Shortcut.Description = "Приложение для изучения с флэш-карточками"
    if (Test-Path "$InstallPath\icon.ico") {
        $Shortcut.IconLocation = "$InstallPath\icon.ico"
    }
    $Shortcut.Save()

    # Создание ярлыка в меню "Пуск"
    Write-Host "Создание ярлыка в меню Пуск..." -ForegroundColor Cyan
    $StartMenuShortcut = $WshShell.CreateShortcut("$StartMenuPath\$ShortcutName.lnk")
    $StartMenuShortcut.TargetPath = "$InstallPath\FlashCardApp.exe"
    $StartMenuShortcut.WorkingDirectory = $InstallPath
    $StartMenuShortcut.Description = "Приложение для изучения с флэш-карточками"
    if (Test-Path "$InstallPath\icon.ico") {
        $StartMenuShortcut.IconLocation = "$InstallPath\icon.ico"
    }
    $StartMenuShortcut.Save()

    # Добавление в список установленных программ
    Write-Host "Регистрация в системе..." -ForegroundColor Cyan
    $UninstallPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\FlashCardApp"
    if (-not (Test-Path $UninstallPath)) {
        New-Item -Path $UninstallPath -Force | Out-Null
    }
    
    Set-ItemProperty -Path $UninstallPath -Name "DisplayName" -Value "FlashCard App"
    Set-ItemProperty -Path $UninstallPath -Name "DisplayVersion" -Value "1.0.0"
    Set-ItemProperty -Path $UninstallPath -Name "Publisher" -Value "FlashCard App Team"
    Set-ItemProperty -Path $UninstallPath -Name "InstallLocation" -Value $InstallPath
    Set-ItemProperty -Path $UninstallPath -Name "UninstallString" -Value "powershell.exe -ExecutionPolicy Bypass -File `"$InstallPath\Uninstall.ps1`""
    Set-ItemProperty -Path $UninstallPath -Name "NoModify" -Value 1
    Set-ItemProperty -Path $UninstallPath -Name "NoRepair" -Value 1
    Set-ItemProperty -Path $UninstallPath -Name "EstimatedSize" -Value 165000

    # Создание деинсталлятора
    Write-Host "Создание деинсталлятора..." -ForegroundColor Cyan
    $UninstallScript = @"
# FlashCard App Uninstaller

Write-Host "=== FlashCard App Uninstaller ===" -ForegroundColor Red
Write-Host ""

`$response = Read-Host "Вы действительно хотите удалить FlashCard App? (Y/N)"
if (`$response -ne "Y" -and `$response -ne "y") {
    Write-Host "Удаление отменено." -ForegroundColor Yellow
    Exit
}

try {
    # Удаление ярлыков
    Write-Host "Удаление ярлыков..." -ForegroundColor Cyan
    Remove-Item "$DesktopPath\$ShortcutName.lnk" -ErrorAction SilentlyContinue
    Remove-Item "$StartMenuPath\$ShortcutName.lnk" -ErrorAction SilentlyContinue

    # Удаление из реестра
    Write-Host "Удаление из реестра..." -ForegroundColor Cyan
    Remove-Item "$UninstallPath" -Recurse -ErrorAction SilentlyContinue

         # Удаление файлов (кроме данных пользователя)
     Write-Host "Удаление файлов..." -ForegroundColor Cyan
     Remove-Item "$InstallPath\FlashCardApp.exe" -ErrorAction SilentlyContinue
     Remove-Item "$InstallPath\FlashCardApp.pdb" -ErrorAction SilentlyContinue
     Remove-Item "$InstallPath\icon.ico" -ErrorAction SilentlyContinue
     Remove-Item "$InstallPath\Uninstall.ps1" -ErrorAction SilentlyContinue
    
    # Удаление каталога, если он пустой
    if ((Get-ChildItem "$InstallPath" -ErrorAction SilentlyContinue).Count -eq 0) {
        Remove-Item "$InstallPath" -ErrorAction SilentlyContinue
    }

    Write-Host ""
    Write-Host "FlashCard App успешно удален!" -ForegroundColor Green
    Write-Host "Данные пользователя сохранены в %APPDATA%\FlashCardApp" -ForegroundColor Yellow
    
} catch {
    Write-Host "Ошибка при удалении: `$_" -ForegroundColor Red
    Exit 1
}

Write-Host ""
Write-Host "Нажмите любую клавишу для продолжения..."
`$null = `$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
"@

    $UninstallScript | Out-File -FilePath "$InstallPath\Uninstall.ps1" -Encoding UTF8

    Write-Host ""
    Write-Host "✅ Установка завершена успешно!" -ForegroundColor Green
    Write-Host ""
    Write-Host "FlashCard App установлен в: $InstallPath" -ForegroundColor Yellow
    Write-Host "Ярлыки созданы на рабочем столе и в меню Пуск" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Для удаления используйте 'Программы и компоненты' в Панели управления" -ForegroundColor Gray
    
    if (-not $Silent) {
        Write-Host ""
        $launch = Read-Host "Запустить FlashCard App сейчас? (Y/N)"
        if ($launch -eq "Y" -or $launch -eq "y") {
            Start-Process "$InstallPath\FlashCardApp.exe"
        }
    }

} catch {
    Write-Host "❌ Ошибка при установке: $_" -ForegroundColor Red
    Exit 1
}

if (-not $Silent) {
    Write-Host ""
    Write-Host "Нажмите любую клавишу для продолжения..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
} 