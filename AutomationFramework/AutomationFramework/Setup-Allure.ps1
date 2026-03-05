# Allure Setup Script for Windows
# This script will help you set up Java (required for Allure) and Allure command-line tool

Write-Host "=== Allure Reporting Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if Java is installed
$javaInstalled = Get-Command java -ErrorAction SilentlyContinue

if (-not $javaInstalled) {
    Write-Host "Java is not installed. Allure requires Java to run." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please install Java using ONE of these methods:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1 - Using Chocolatey:" -ForegroundColor Green
    Write-Host "  choco install openjdk17" -ForegroundColor White
    Write-Host ""
    Write-Host "Option 2 - Using Winget:" -ForegroundColor Green
    Write-Host "  winget install Microsoft.OpenJDK.17" -ForegroundColor White
    Write-Host ""
    Write-Host "Option 3 - Manual Download:" -ForegroundColor Green
    Write-Host "  Download from: https://adoptium.net/" -ForegroundColor White
    Write-Host ""
    Write-Host "After installing Java, close and reopen PowerShell, then run this script again." -ForegroundColor Yellow
    exit
}

Write-Host "✓ Java is installed: $(java -version 2>&1 | Select-Object -First 1)" -ForegroundColor Green
Write-Host ""

# Check if Allure is installed
$allureInstalled = Get-Command allure -ErrorAction SilentlyContinue

if (-not $allureInstalled) {
    # Check if Allure was extracted to user profile
    $allurePath = "$env:USERPROFILE\allure\allure-2.30.0\bin"
    
    if (Test-Path $allurePath) {
        Write-Host "Allure found at: $allurePath" -ForegroundColor Green
        Write-Host "Adding Allure to PATH for this session..." -ForegroundColor Yellow
        $env:Path += ";$allurePath"
        
        Write-Host ""
        Write-Host "To make this permanent, add Allure to your System Environment Variables:" -ForegroundColor Yellow
        Write-Host "  Path: $allurePath" -ForegroundColor White
        Write-Host ""
        Write-Host "Or run this command as Administrator:" -ForegroundColor Yellow
        Write-Host "  [Environment]::SetEnvironmentVariable('Path', `$env:Path + ';$allurePath', 'Machine')" -ForegroundColor White
    }
    else {
        Write-Host "Allure is not installed." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Install Allure using ONE of these methods:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Option 1 - Using Chocolatey (Recommended):" -ForegroundColor Green
        Write-Host "  choco install allure" -ForegroundColor White
        Write-Host ""
        Write-Host "Option 2 - Using Winget:" -ForegroundColor Green
        Write-Host "  winget install allure" -ForegroundColor White
        Write-Host ""
        Write-Host "Option 3 - Already downloaded:" -ForegroundColor Green
        Write-Host "  Allure has been downloaded to your user profile" -ForegroundColor White
        Write-Host "  Just add it to PATH (instructions above)" -ForegroundColor White
        exit
    }
}

Write-Host "✓ Allure is ready: $(allure --version)" -ForegroundColor Green
Write-Host ""

# Show usage instructions
Write-Host "=== Quick Start Guide ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Run your tests:" -ForegroundColor Yellow
Write-Host "   dotnet test" -ForegroundColor White
Write-Host ""
Write-Host "2. Generate and view Allure report:" -ForegroundColor Yellow
Write-Host "   cd AutomationFramework\bin\Debug\net10.0" -ForegroundColor White
Write-Host "   allure serve allure-results" -ForegroundColor White
Write-Host ""
Write-Host "3. Or generate report to folder:" -ForegroundColor Yellow
Write-Host "   allure generate allure-results --clean -o allure-report" -ForegroundColor White
Write-Host "   allure open allure-report" -ForegroundColor White
Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Green
