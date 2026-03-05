@echo off
echo.
echo ========================================
echo    Allure Report Generator
echo ========================================
echo.

REM Check if allure-results exists
if not exist "allure-results" (
    echo ERROR: allure-results folder not found!
    echo Please run your tests first: dotnet test
    echo.
    pause
    exit /b 1
)

echo Generating and opening Allure report...
echo.

allure serve allure-results

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Failed to generate report.
    echo Please ensure Java and Allure are installed.
    echo Run Setup-Allure.ps1 for installation help.
    echo.
    pause
)
