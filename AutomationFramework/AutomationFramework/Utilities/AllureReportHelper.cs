using Allure.Net.Commons;
using OpenQA.Selenium;
using System;
using System.IO;

namespace TestProject1.Utilities
{
    public static class AllureReportHelper
    {
        /// <summary>
        /// Log a step in the test execution
        /// </summary>
        public static void LogStep(string stepDescription)
        {
            AllureApi.Step(stepDescription);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        public static void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void LogWarning(string message)
        {
            Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        /// <summary>
        /// Attach a screenshot to the Allure report
        /// </summary>
        public static void AttachScreenshot(IWebDriver driver, string screenshotName = "Screenshot")
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                byte[] screenshotBytes = screenshot.AsByteArray;
                
                AllureApi.AddAttachment(
                    screenshotName,
                    "image/png",
                    screenshotBytes,
                    ".png"
                );
                
                LogInfo($"Screenshot '{screenshotName}' attached to report");
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to attach screenshot: {ex.Message}");
            }
        }

        /// <summary>
        /// Attach text content to the Allure report
        /// </summary>
        public static void AttachText(string name, string content)
        {
            try
            {
                AllureApi.AddAttachment(name, "text/plain", content);
                LogInfo($"Text attachment '{name}' added to report");
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to attach text: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a parameter to the current test
        /// </summary>
        public static void AddParameter(string name, string value)
        {
            AllureLifecycle.Instance.UpdateTestCase(tc => tc.parameters.Add(new Parameter
            {
                name = name,
                value = value
            }));
        }

        /// <summary>
        /// Set test description
        /// </summary>
        public static void SetDescription(string description)
        {
            AllureLifecycle.Instance.UpdateTestCase(tc => tc.description = description);
        }

        /// <summary>
        /// Add a link to the test (e.g., Jira ticket, documentation)
        /// </summary>
        public static void AddLink(string name, string url)
        {
            AllureLifecycle.Instance.UpdateTestCase(tc => tc.links.Add(new Link
            {
                name = name,
                url = url
            }));
        }

        /// <summary>
        /// Set test severity
        /// </summary>
        public static void SetSeverity(SeverityLevel severity)
        {
            AllureLifecycle.Instance.UpdateTestCase(tc => tc.labels.Add(new Label
            {
                name = "severity",
                value = severity.ToString().ToLower()
            }));
        }
    }
}
