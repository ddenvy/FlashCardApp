@echo off
REM FlashCard App - Create Portable Version
REM Создание портативной версии приложения

title FlashCard App - Create Portable Version

echo ===========================================
echo   FlashCard App - Create Portable Version
echo ===========================================
echo.

REM Переход в корневую папку проекта
cd /d "%~dp0.."

REM Проверка наличия опубликованного приложения
if not exist "publish\FlashCardApp.exe" (
    echo ОШИБКА: Опубликованное приложение не найдено!
    echo Сначала запустите Build-Installer.bat или выполните:
    echo dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
    pause
    exit /b 1
)

echo 1. Создание папки для портативной версии...
set PORTABLE_DIR=FlashCardApp-Portable
if exist "%PORTABLE_DIR%" rmdir /s /q "%PORTABLE_DIR%"
mkdir "%PORTABLE_DIR%"

echo 2. Копирование исполняемого файла...
copy "publish\FlashCardApp.exe" "%PORTABLE_DIR%\" >nul
if %errorlevel% neq 0 (
    echo ОШИБКА: Не удалось скопировать FlashCardApp.exe!
    pause
    exit /b 1
)

echo 3. Создание файла portable.txt...
echo Этот файл указывает приложению работать в портативном режиме. > "%PORTABLE_DIR%\portable.txt"
echo В портативном режиме все данные сохраняются в папке приложения. >> "%PORTABLE_DIR%\portable.txt"
echo. >> "%PORTABLE_DIR%\portable.txt"
echo Для обновления замените FlashCardApp.exe на новую версию. >> "%PORTABLE_DIR%\portable.txt"

echo 4. Копирование документации и ресурсов...
copy "README.md" "%PORTABLE_DIR%\" >nul 2>&1
copy "LICENSE.txt" "%PORTABLE_DIR%\" >nul 2>&1
copy "assets\icon.ico" "%PORTABLE_DIR%\" >nul 2>&1

echo 5. Создание README для портативной версии...
(
echo # FlashCard App - Портативная версия
echo.
echo ## Что это?
echo.
echo Это портативная версия FlashCard App, которая не требует установки.
echo Все данные и настройки сохраняются в папке приложения.
echo.
echo ## Как использовать
echo.
echo 1. Запустите FlashCardApp.exe
echo 2. Все карточки и настройки будут сохранены в папке Data
echo 3. Для переноса на другой компьютер скопируйте всю папку
echo.
echo ## Файлы
echo.
echo - `FlashCardApp.exe` - основное приложение
echo - `portable.txt` - файл, указывающий на портативный режим
echo - `Data/` - папка с данными пользователя ^(создается автоматически^)
echo - `README-Portable.md` - этот файл
echo.
echo ## Системные требования
echo.
echo - Windows 10/11 ^(x64^)
echo - Не требует установки .NET Framework
echo.
echo ## Обновление
echo.
echo Для обновления замените FlashCardApp.exe на новую версию.
echo Данные пользователя останутся без изменений.
echo.
echo ## Удаление
echo.
echo Просто удалите папку FlashCardApp-Portable.
echo Никаких следов в системе не останется.
) > "%PORTABLE_DIR%\README-Portable.md"

echo 6. Создание bat файла для запуска...
(
echo @echo off
echo REM FlashCard App Portable Launcher
echo.
echo title FlashCard App
echo.
echo REM Проверка наличия основного файла
echo if not exist "FlashCardApp.exe" ^(
echo     echo ОШИБКА: FlashCardApp.exe не найден!
echo     pause
echo     exit /b 1
echo ^)
echo.
echo REM Запуск приложения
echo start "" "FlashCardApp.exe"
) > "%PORTABLE_DIR%\Запуск FlashCard App.bat"

echo 7. Получение информации о размере...
for %%A in ("%PORTABLE_DIR%\FlashCardApp.exe") do set size=%%~zA
set /a sizeMB=%size% / 1024 / 1024

echo 8. Создание архива (если доступен)...
where 7z >nul 2>nul
if %errorlevel% equ 0 (
    echo    Создание ZIP архива с помощью 7-Zip...
    7z a -tzip "FlashCardApp-Portable.zip" "%PORTABLE_DIR%\*" >nul
    if %errorlevel% equ 0 (
        echo    ✓ Архив FlashCardApp-Portable.zip создан
    ) else (
        echo    ✗ Ошибка при создании архива
    )
) else (
    where powershell >nul 2>nul
    if %errorlevel% equ 0 (
        echo    Создание ZIP архива с помощью PowerShell...
        powershell -command "Compress-Archive -Path '%PORTABLE_DIR%\*' -DestinationPath 'FlashCardApp-Portable.zip' -Force"
        if %errorlevel% equ 0 (
            echo    ✓ Архив FlashCardApp-Portable.zip создан
        ) else (
            echo    ✗ Ошибка при создании архива
        )
    ) else (
        echo    Архиватор не найден. Архив не создан.
    )
)

echo.
echo ===========================================
echo      Портативная версия готова!
echo ===========================================
echo.
echo Результаты:
echo   ✓ Папка: %PORTABLE_DIR%
echo   ✓ Размер приложения: %sizeMB% МБ
echo   ✓ Файлы готовы к использованию
echo.
echo Содержимое:
echo   - FlashCardApp.exe (основное приложение)
echo   - portable.txt (маркер портативного режима)
echo   - README-Portable.md (инструкции)
echo   - LICENSE.txt (лицензия)
echo   - "Запуск FlashCard App.bat" (ярлык для запуска)
echo.

if exist "FlashCardApp-Portable.zip" (
    echo   ✓ Архив: FlashCardApp-Portable.zip
    echo.
)

echo Для использования:
echo   1. Скопируйте папку %PORTABLE_DIR% в любое место
echo   2. Запустите FlashCardApp.exe или "Запуск FlashCard App.bat"
echo.

set /p run="Запустить портативную версию сейчас? (Y/N): "
if /i "%run%"=="Y" (
    start "" "%PORTABLE_DIR%\FlashCardApp.exe"
)

echo.
echo Готово! Нажмите любую клавишу для завершения...
pause >nul 