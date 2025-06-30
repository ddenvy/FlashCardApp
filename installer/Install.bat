@echo off
REM FlashCard App Installer
REM Запуск PowerShell скрипта установки

title FlashCard App Installer

echo ============================================
echo        FlashCard App Installer
echo ============================================
echo.

REM Проверка наличия PowerShell
where powershell >nul 2>nul
if %errorlevel% neq 0 (
    echo ОШИБКА: PowerShell не найден в системе!
    echo Установите PowerShell для продолжения.
    pause
    exit /b 1
)

REM Проверка наличия файла установки
if not exist "%~dp0Install-FlashCardApp.ps1" (
    echo ОШИБКА: Файл Install-FlashCardApp.ps1 не найден!
    echo Убедитесь, что все файлы инсталлятора находятся в одной папке.
    pause
    exit /b 1
)

REM Проверка наличия исполняемого файла
if not exist "%~dp0..\publish\FlashCardApp.exe" (
    echo ОШИБКА: FlashCardApp.exe не найден!
    echo Убедитесь, что приложение было собрано в папке publish.
    pause
    exit /b 1
)

echo Запуск установки FlashCard App...
echo.
echo ВНИМАНИЕ: Для установки требуются права администратора!
echo Если появится запрос UAC, нажмите "Да" для продолжения.
echo.
pause

REM Запуск PowerShell скрипта с правами администратора
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0Install-FlashCardApp.ps1'"

if %errorlevel% equ 0 (
    echo.
    echo ============================================
    echo     Установка выполнена успешно!
    echo ============================================
) else (
    echo.
    echo ============================================
    echo      Произошла ошибка при установке
    echo ============================================
)

echo.
pause 