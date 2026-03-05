using OpenQA.Selenium;
using TestProject1.Locators;

namespace TestProject1.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Perform login with given username and password
        public void Login(string username, string password)
        {
            driver.FindElement(LoginPageLocators.UsernameField).Clear();
            driver.FindElement(LoginPageLocators.UsernameField).SendKeys(username);

            driver.FindElement(LoginPageLocators.PasswordField).Clear();
            driver.FindElement(LoginPageLocators.PasswordField).SendKeys(password);

            driver.FindElement(LoginPageLocators.LoginButton).Click();
        }

        // Get error message for invalid login
        public string GetErrorMessage()
        {
            return driver.FindElement(LoginPageLocators.ErrorMessage).Text;
        }
    }
}
