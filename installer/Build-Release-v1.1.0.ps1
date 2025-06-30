# FlashCard App - Сборка релиза v1.1.0
# PowerShell скрипт для автоматической сборки

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FlashCard App - Сборка релиза v1.1.0" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Переход в корневую папку проекта
$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $projectRoot

try {
    # 1. Очистка предыдущих сборок
    Write-Host "1. Очистка предыдущих сборок..." -ForegroundColor Yellow
    
    $foldersToRemove = @("bin\Release", "obj\Release", "publish")
    foreach ($folder in $foldersToRemove) {
        if (Test-Path $folder) {
            Remove-Item $folder -Recurse -Force
            Write-Host "   Удалена папка: $folder" -ForegroundColor DarkGray
        }
    }
    
    if (Test-Path "dist\FlashCardApp-Setup-v1.1.0.exe") {
        Remove-Item "dist\FlashCardApp-Setup-v1.1.0.exe" -Force
        Write-Host "   Удален старый установщик" -ForegroundColor DarkGray
    }

    # 2. Сборка проекта
    Write-Host ""
    Write-Host "2. Сборка проекта в Release режиме..." -ForegroundColor Yellow
    
    & dotnet build --configuration Release --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "Ошибка при сборке проекта"
    }
    Write-Host "   ✅ Сборка завершена успешно" -ForegroundColor Green

    # 3. Публикация приложения
    Write-Host ""
    Write-Host "3. Публикация приложения..." -ForegroundColor Yellow
    
    & dotnet publish --configuration Release --output publish --self-contained false --runtime win-x64 --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "Ошибка при публикации приложения"
    }
    Write-Host "   ✅ Публикация завершена успешно" -ForegroundColor Green

    # 4. Копирование дополнительных файлов
    Write-Host ""
    Write-Host "4. Копирование дополнительных файлов..." -ForegroundColor Yellow
    
    $filesToCopy = @{
        "assets\icon.ico" = "publish\icon.ico"
        "README.md" = "publish\README.md"
        "LICENSE.txt" = "publish\LICENSE.txt"
        "RELEASE-NOTES-v1.1.0.md" = "publish\RELEASE-NOTES-v1.1.0.md"
    }
    
    foreach ($source in $filesToCopy.Keys) {
        if (Test-Path $source) {
            Copy-Item $source $filesToCopy[$source] -Force
            Write-Host "   Скопирован: $source" -ForegroundColor DarkGray
        } else {
            Write-Host "   ⚠️ Файл не найден: $source" -ForegroundColor Yellow
        }
    }

    # 5. Создание папки dist
    Write-Host ""
    Write-Host "5. Создание папки для релиза..." -ForegroundColor Yellow
    
    if (!(Test-Path "dist")) {
        New-Item -ItemType Directory -Path "dist" | Out-Null
        Write-Host "   Создана папка: dist" -ForegroundColor DarkGray
    }

    # 6. Проверка Inno Setup и создание установщика
    Write-Host ""
    Write-Host "6. Создание установщика..." -ForegroundColor Yellow
    
    $innoSetupPaths = @(
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    )
    
    $innoSetupPath = $null
    foreach ($path in $innoSetupPaths) {
        if (Test-Path $path) {
            $innoSetupPath = $path
            break
        }
    }
    
    if ($innoSetupPath) {
        Write-Host "   Найден Inno Setup: $innoSetupPath" -ForegroundColor DarkGray
        
        & "$innoSetupPath" "installer\FlashCardApp-Setup.iss"
        if ($LASTEXITCODE -eq 0) {
            Write-Host "   ✅ Установщик создан успешно" -ForegroundColor Green
        } else {
            throw "Ошибка при создании установщика"
        }
    } else {
        Write-Host "   ⚠️ Inno Setup не найден!" -ForegroundColor Red
        Write-Host "   Загрузите и установите Inno Setup 6 с: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
        Write-Host "   Затем создайте установщик вручную:" -ForegroundColor Yellow
        Write-Host "   1. Откройте installer\FlashCardApp-Setup.iss в Inno Setup" -ForegroundColor Yellow
        Write-Host "   2. Нажмите Build -> Compile" -ForegroundColor Yellow
    }

    # Итоговый отчет
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "✅ СБОРКА РЕЛИЗА ЗАВЕРШЕНА УСПЕШНО!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    
    Write-Host "📁 Файлы созданы:" -ForegroundColor Cyan
    if (Test-Path "publish\FlashCardApp.exe") {
        $publishSize = [math]::Round((Get-Item "publish\FlashCardApp.exe").Length / 1MB, 2)
        Write-Host "   - Приложение: publish\FlashCardApp.exe ($publishSize МБ)" -ForegroundColor White
    }
    
    if (Test-Path "dist\FlashCardApp-Setup-v1.1.0.exe") {
        $installerSize = [math]::Round((Get-Item "dist\FlashCardApp-Setup-v1.1.0.exe").Length / 1MB, 2)
        Write-Host "   - Установщик: dist\FlashCardApp-Setup-v1.1.0.exe ($installerSize МБ)" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "🚀 Следующие шаги:" -ForegroundColor Cyan
    Write-Host "   1. Протестируйте установщик на чистой системе" -ForegroundColor White
    Write-Host "   2. Создайте релиз на GitHub (https://github.com/ddenvy/FlashCardApp/releases/new)" -ForegroundColor White
    Write-Host "   3. Загрузите FlashCardApp-Setup-v1.1.0.exe как assets" -ForegroundColor White
    Write-Host "   4. Скопируйте содержимое RELEASE-NOTES-v1.1.0.md в описание релиза" -ForegroundColor White
    Write-Host "   5. Установите тег версии: v1.1.0" -ForegroundColor White

} catch {
    Write-Host ""
    Write-Host "❌ ОШИБКА: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Сборка прервана." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Нажмите любую клавишу для выхода..." -ForegroundColor DarkGray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 