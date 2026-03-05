using OpenQA.Selenium;
using TestProject1.Locators;

namespace TestProject1.Pages
{
    public class CheckoutPage
    {
        private IWebDriver driver;

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Fill checkout information and click Continue
        public void EnterCheckoutInfo(string firstName, string lastName, string postalCode)
        {
            driver.FindElement(CheckoutPageLocators.FirstNameField).SendKeys(firstName);
            driver.FindElement(CheckoutPageLocators.LastNameField).SendKeys(lastName);
            driver.FindElement(CheckoutPageLocators.PostalCodeField).SendKeys(postalCode);
            driver.FindElement(CheckoutPageLocators.ContinueButton).Click();
        }

        // Click Finish button to complete order
        public void FinishOrder()
        {
            driver.FindElement(CheckoutPageLocators.FinishButton).Click();
        }

        // Get the success message after order completion
        public string GetSuccessMessage()
        {
            return driver.FindElement(CheckoutPageLocators.CompleteHeader).Text;
        }

        // Navigate back to inventory/home page
        public void BackToHome()
        {
            driver.FindElement(CheckoutPageLocators.BackToProductsButton).Click();
        }
    }
}
