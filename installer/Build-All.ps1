param(
    [string]$Version = "2.0.0",
    [string]$Platform = "All"
)

Write-Host "QuickMind Universal Installer Builder v$Version" -ForegroundColor Cyan
Write-Host "Platform: $Platform" -ForegroundColor Yellow

$ErrorActionPreference = "Continue"

# Detect current platform
$IsWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)
$IsMacOS = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)

Write-Host "Current OS: Windows=$IsWindows, macOS=$IsMacOS" -ForegroundColor Gray

# Check .NET version
$dotnetVersion = dotnet --version
Write-Host ".NET Version: $dotnetVersion" -ForegroundColor Gray

# Function to build Windows installer
function Build-WindowsInstaller {
    Write-Host "Building Windows Installer..." -ForegroundColor Blue
    
    if (Test-Path ".\installer\Build-Windows-Installer.ps1") {
        & .\installer\Build-Windows-Installer.ps1 -Version $Version
        return $LASTEXITCODE -eq 0
    } else {
        Write-Host "Windows installer script not found!" -ForegroundColor Red
        return $false
    }
}

# Function to build macOS installer  
function Build-macOSInstaller {
    Write-Host "Building macOS Installer..." -ForegroundColor Blue
    
    if ($IsMacOS) {
        if (Test-Path "./installer/Build-macOS-Installer.sh") {
            bash ./installer/Build-macOS-Installer.sh
            return $LASTEXITCODE -eq 0
        } else {
            Write-Host "macOS installer script not found!" -ForegroundColor Red
            return $false
        }
    } else {
        Write-Host "macOS installer can only be built on macOS" -ForegroundColor Yellow
        
        # Build for macOS but don't create DMG
        Write-Host "Building macOS binary..." -ForegroundColor Yellow
        dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=false -o "./publish/osx-x64"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "macOS binary built successfully in ./publish/osx-x64" -ForegroundColor Green
            Write-Host "Transfer files to macOS to complete .app bundle and DMG creation" -ForegroundColor Cyan
            return $true
        } else {
            return $false
        }
    }
}

# Main build logic
$results = @()

switch ($Platform.ToLower()) {
    "windows" {
        $results += Build-WindowsInstaller
    }
    "macos" {
        $results += Build-macOSInstaller
    }
    "all" {
        if ($IsWindows -or $Platform -eq "All") {
            $results += Build-WindowsInstaller
        }
        
        $results += Build-macOSInstaller
    }
    default {
        Write-Host "Unknown platform: $Platform" -ForegroundColor Red
        Write-Host "Valid platforms: Windows, macOS, All" -ForegroundColor Yellow
        exit 1
    }
}

# Summary
Write-Host "Build Summary:" -ForegroundColor Cyan
$successCount = ($results | Where-Object { $_ -eq $true }).Count
$totalCount = $results.Count

if ($successCount -eq $totalCount -and $totalCount -gt 0) {
    Write-Host "All builds completed successfully! ($successCount/$totalCount)" -ForegroundColor Green
} elseif ($successCount -gt 0) {
    Write-Host "Partial success: $successCount/$totalCount builds completed" -ForegroundColor Yellow
} else {
    Write-Host "All builds failed!" -ForegroundColor Red
    exit 1
}

# Show output files
Write-Host "Output files:" -ForegroundColor Cyan
if (Test-Path "./dist") {
    Get-ChildItem "./dist" -File | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "  $($_.Name) ($size MB)" -ForegroundColor White
    }
} else {
    Write-Host "  No output files found in ./dist" -ForegroundColor Gray
}

Write-Host "Build process completed!" -ForegroundColor Green 