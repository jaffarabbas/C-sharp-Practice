using NUnit.Framework;
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

        [SetUp] // runs once per test class
        public void GlobalSetup()
        {
            // 1️⃣ Initialize WebDriver
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);

            // 2️⃣ Initialize Wait
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 3️⃣ Load JSON test data (absolute path or dynamic path)
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "loginData.json");
            if (!File.Exists(jsonPath))
            {
                // fallback to absolute path if file not found in output folder
                jsonPath = @"C:\Users\Fast\source\repos\TestProject1\TestData\loginData.json";
            }

            testData = JsonHelper.ReadJson<LoginData>(jsonPath);

            // 4️⃣ Navigate to base URL
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        }

        [TearDown] // runs once after all tests
        public void GlobalTearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null!;
            }
        }
    }
}
