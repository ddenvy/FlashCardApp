@echo off
REM FlashCard App - Build and Package Script
REM Автоматическая сборка приложения и создание инсталлятора

title FlashCard App - Build and Package

echo ===========================================
echo     FlashCard App - Build and Package
echo ===========================================
echo.

REM Переход в корневую папку проекта
cd /d "%~dp0.."

REM Проверка наличия project файла
if not exist "FlashCardApp.csproj" (
    echo ОШИБКА: FlashCardApp.csproj не найден!
    echo Убедитесь, что скрипт запущен из папки installer.
    pause
    exit /b 1
)

echo 1. Очистка предыдущих сборок...
if exist "bin" rmdir /s /q bin
if exist "obj" rmdir /s /q obj
if exist "publish" rmdir /s /q publish
if exist "dist" rmdir /s /q dist

echo 2. Восстановление пакетов...
dotnet restore
if %errorlevel% neq 0 (
    echo ОШИБКА: Не удалось восстановить пакеты!
    pause
    exit /b 1
)

echo 3. Сборка проекта...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo ОШИБКА: Не удалось собрать проект!
    pause
    exit /b 1
)

echo 4. Публикация single-file executable...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
if %errorlevel% neq 0 (
    echo ОШИБКА: Не удалось опубликовать приложение!
    pause
    exit /b 1
)

echo 5. Проверка результатов публикации...
if not exist "publish\FlashCardApp.exe" (
    echo ОШИБКА: FlashCardApp.exe не был создан!
    pause
    exit /b 1
)

REM Получение размера файла
for %%A in ("publish\FlashCardApp.exe") do set size=%%~zA
set /a sizeMB=%size% / 1024 / 1024
echo    ✓ FlashCardApp.exe создан (размер: %sizeMB% МБ)

echo 6. Создание папок для инсталлятора...
if not exist "dist" mkdir dist
if not exist "assets" mkdir assets

echo 7. Копирование дополнительных файлов...
copy README.md publish\ >nul 2>&1
copy LICENSE.txt publish\ >nul 2>&1
copy assets\icon.ico publish\ >nul 2>&1

echo.
echo ===========================================
echo          Сборка завершена успешно!
echo ===========================================
echo.
echo Результаты:
echo   ✓ Исполняемый файл: publish\FlashCardApp.exe
echo   ✓ Размер: %sizeMB% МБ
echo   ✓ Инсталляторы готовы к созданию
echo.

REM Проверка наличия Inno Setup
where iscc >nul 2>nul
if %errorlevel% equ 0 (
    echo Найден Inno Setup Compiler!
    echo.
    set /p choice="Создать Inno Setup инсталлятор? (Y/N): "
    if /i "%choice%"=="Y" (
        echo 8. Создание Inno Setup инсталлятора...
        iscc "installer\FlashCardApp-Setup.iss"
        if %errorlevel% equ 0 (
            echo    ✓ Inno Setup инсталлятор создан в папке dist
        ) else (
            echo    ✗ Ошибка при создании Inno Setup инсталлятора
        )
    )
) else (
    echo Inno Setup не найден. Для создания профессионального инсталлятора:
    echo 1. Скачайте Inno Setup: https://jrsoftware.org/isinfo.php
    echo 2. Откройте installer\FlashCardApp-Setup.iss
    echo 3. Нажмите Build - Compile
)

echo.
echo Доступные инсталляторы:
echo   1. PowerShell: installer\Install.bat
echo   2. Inno Setup: installer\FlashCardApp-Setup.iss
echo.

set /p run="Запустить PowerShell инсталлятор сейчас? (Y/N): "
if /i "%run%"=="Y" (
    cd installer
    call Install.bat
)

echo.
echo Готово! Нажмите любую клавишу для завершения...
pause >nul 