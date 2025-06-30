@echo off
echo ========================================
echo FlashCard App - Сборка релиза v1.1.0
echo ========================================

echo.
echo 1. Очистка предыдущих сборок...
cd /d "%~dp0.."
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "obj\Release" rmdir /s /q "obj\Release"
if exist "publish" rmdir /s /q "publish"
if exist "dist\FlashCardApp-Setup-v1.1.0.exe" del "dist\FlashCardApp-Setup-v1.1.0.exe"

echo.
echo 2. Сборка проекта в Release режиме...
dotnet build --configuration Release
if errorlevel 1 (
    echo ОШИБКА: Не удалось собрать проект!
    pause
    exit /b 1
)

echo.
echo 3. Публикация приложения...
dotnet publish --configuration Release --output publish --self-contained false --runtime win-x64
if errorlevel 1 (
    echo ОШИБКА: Не удалось опубликовать приложение!
    pause
    exit /b 1
)

echo.
echo 4. Копирование дополнительных файлов...
copy "assets\icon.ico" "publish\" >nul
copy "README.md" "publish\" >nul
copy "LICENSE.txt" "publish\" >nul

echo.
echo 5. Создание папки dist...
if not exist "dist" mkdir "dist"

echo.
echo 6. Создание установщика с помощью Inno Setup...
echo Запуск: "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\FlashCardApp-Setup.iss

"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\FlashCardApp-Setup.iss
if errorlevel 1 (
    echo ПРЕДУПРЕЖДЕНИЕ: Не удалось найти Inno Setup или произошла ошибка
    echo Проверьте, что Inno Setup 6 установлен в: C:\Program Files (x86)\Inno Setup 6\
    echo.
    echo Вы можете создать установщик вручную:
    echo 1. Откройте installer\FlashCardApp-Setup.iss в Inno Setup
    echo 2. Нажмите Build -^> Compile
    pause
    exit /b 1
)

echo.
echo ========================================
echo ✅ СБОРКА РЕЛИЗА ЗАВЕРШЕНА УСПЕШНО!
echo ========================================
echo.
echo Файлы созданы:
echo - Скомпилированное приложение: publish\FlashCardApp.exe
echo - Установщик: dist\FlashCardApp-Setup-v1.1.0.exe
echo.
echo Следующие шаги:
echo 1. Протестируйте установщик
echo 2. Создайте релиз на GitHub
echo 3. Загрузите FlashCardApp-Setup-v1.1.0.exe
echo 4. Опубликуйте примечания к релизу
echo.

pause 