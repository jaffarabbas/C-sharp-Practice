using OpenQA.Selenium;
using TestProject1.Locators;

namespace TestProject1.Pages
{
    public class InventoryPage
    {
        private IWebDriver driver;

        public InventoryPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Add the Backpack product to the cart
        public void AddBackpackToCart()
        {
            driver.FindElement(InventoryPageLocators.BackpackAddButton).Click();
        }

        // Open the cart page by clicking the cart link
        public void OpenCart()
        {
            driver.FindElement(InventoryPageLocators.CartLink).Click();
        }
    }
}
