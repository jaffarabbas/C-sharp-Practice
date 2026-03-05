# Allure Reporting Setup

## Overview
This test framework now includes Allure reporting for comprehensive test execution reports with screenshots, steps, and detailed test information.

## Installation

The Allure.NUnit package has been added to the project. To install Allure command-line tool:

### Windows (Using Scoop)
```powershell
scoop install allure
```

### Windows (Manual)
1. Download Allure from: https://github.com/allure-framework/allure2/releases
2. Extract and add to PATH

### macOS (Using Homebrew)
```bash
brew install allure
```

## Running Tests

Run your tests as usual:
```powershell
dotnet test
```

Test results will be saved in the `allure-results` folder in your bin directory.

## Generating Allure Report

After running tests, generate the HTML report:

```powershell
allure generate allure-results --clean -o allure-report
```

## Viewing the Report

Open the report in your browser:
```powershell
allure serve allure-results
```

Or open the generated report manually:
```powershell
allure open allure-report
```

## Features Implemented

### 1. Centralized Allure Helper (`AllureReportHelper.cs`)
Located in `Utilities` folder with methods for:
- **LogStep()** - Log test execution steps
- **LogInfo()** - Log informational messages
- **LogWarning()** - Log warnings
- **AttachScreenshot()** - Capture and attach screenshots
- **AttachText()** - Attach text content to reports
- **AddParameter()** - Add test parameters
- **SetDescription()** - Set test descriptions
- **AddLink()** - Add links (Jira, docs, etc.)
- **SetSeverity()** - Set test severity

### 2. BaseTest Integration
- **OneTimeSetUp**: Initializes allure-results directory
- **SetUp**: Logs WebDriver initialization and navigation
- **TearDown**: Automatically captures screenshots on test failures and logs test results

### 3. Test Annotations
Each test includes:
- `[AllureTag]` - Categorize tests (Smoke, Regression, E2E, etc.)
- `[AllureSeverity]` - Set test priority (blocker, critical, normal, etc.)
- `[AllureDescription]` - Detailed test description
- `[AllureSuite]` - Test suite grouping

### 4. Step Logging
Important points logged in each test:
- Login actions
- Navigation steps
- User interactions
- Verification steps
- Screenshots at key moments

## Report Features

The Allure report provides:
- ✅ Test execution overview with pass/fail statistics
- 📊 Test categorization by tags and severity
- 📸 Screenshots on failures (and important steps)
- 📝 Detailed step-by-step execution logs
- ⏱️ Execution time for each test
- 🔍 Test history and trends
- 📋 Test parameters and configurations

## Best Practices

1. **Use descriptive step names** - Makes reports easier to understand
2. **Capture screenshots at important points** - Not just on failures
3. **Tag tests appropriately** - Helps with filtering and organization
4. **Set appropriate severity levels** - Prioritize test failures
5. **Add parameters for data-driven tests** - Track which data caused issues

## Example Usage in Tests

```csharp
[Test]
[AllureTag("Login", "Smoke")]
[AllureSeverity(SeverityLevel.critical)]
[AllureDescription("Verify successful login")]
public void LoginTest()
{
    AllureReportHelper.LogStep("Starting login test");
    // ... test code ...
    AllureReportHelper.AttachScreenshot(driver, "After Login");
}
```
