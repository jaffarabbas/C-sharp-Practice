using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using TestProject1.TestData;
using TestProject1.Utilities;

namespace TestProject1
{
    public class BaseTest
    {
        protected IWebDriver driver = null!;
        protected WebDriverWait wait = null!;
        protected LoginData testData = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Configure Allure results directory
            string allureResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allure-results");
            if (!Directory.Exists(allureResultsPath))
            {
                Directory.CreateDirectory(allureResultsPath);
            }

            AllureReportHelper.LogInfo("Test suite initialized");
        }

        [SetUp] // runs once per test class
        public void GlobalSetup()
        {
            // 1️⃣ Initialize WebDriver
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);

            AllureReportHelper.LogInfo("WebDriver initialized");

            // 2️⃣ Initialize Wait
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 3️⃣ Load JSON test data (absolute path or dynamic path)
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "loginData.json");
            if (!File.Exists(jsonPath))
            {
                // fallback to project directory path
                string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
                jsonPath = Path.Combine(projectRoot, "TestData", "loginData.json");
            }

            testData = JsonHelper.ReadJson<LoginData>(jsonPath);
            AllureReportHelper.LogInfo("Test data loaded successfully");

            // 4️⃣ Navigate to base URL
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            AllureReportHelper.LogStep("Navigated to Sauce Demo application");
        }

        [TearDown] // runs once after all tests
        public void GlobalTearDown()
        {
            // Capture screenshot on test failure
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                AllureReportHelper.LogWarning($"Test failed: {TestContext.CurrentContext.Result.Message}");
                AllureReportHelper.AttachScreenshot(driver, "Failure Screenshot");

                // Attach additional failure info
                AllureReportHelper.AttachText("Failure Details", 
                    $"Test: {TestContext.CurrentContext.Test.Name}\n" +
                    $"Status: {TestContext.CurrentContext.Result.Outcome.Status}\n" +
                    $"Message: {TestContext.CurrentContext.Result.Message}\n" +
                    $"Stack Trace: {TestContext.CurrentContext.Result.StackTrace}");
            }
            else if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed)
            {
                AllureReportHelper.LogInfo("Test passed successfully");
            }

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null!;
                AllureReportHelper.LogInfo("WebDriver closed");
            }
        }
    }
}
