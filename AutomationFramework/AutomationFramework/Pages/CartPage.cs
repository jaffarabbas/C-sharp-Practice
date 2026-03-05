using OpenQA.Selenium;
using TestProject1.Locators;

namespace TestProject1.Pages
{
    public class CartPage
    {
        private IWebDriver driver;

        public CartPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Click the Checkout button
        public void ClickCheckout()
        {
            driver.FindElement(CartPageLocators.CheckoutButton).Click();
        }

        // Get the first item's name in the cart
        public string GetFirstCartItemName()
        {
            return driver.FindElement(CartPageLocators.CartItemNames).Text;
        }

        // Get the number shown on the cart badge
        public string GetCartBadgeCount()
        {
            return driver.FindElement(CartPageLocators.CartBadge).Text;
        }
    }
}
