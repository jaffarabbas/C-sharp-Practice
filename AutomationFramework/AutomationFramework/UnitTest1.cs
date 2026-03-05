using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace TestProject1
{
    public class Tests
    {
        private IWebDriver driver = null!;
        private WebDriverWait wait = null!;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        }

        // Successful Login Test
        [Test]
        public void LoginSuccessTest()
        {
            Login("standard_user", "secret_sauce");

            wait!.Until(d => d.Url.Contains("inventory"));
            Assert.That(driver!.Url, Does.Contain("inventory"));
        }

        // Login Fail Test (Proper Version)
        [Test]
        public void LoginFailTest()
        {
            Login("standard_user", "wrong_password");

            // Wait for error message
            IWebElement errorMessage = wait!.Until(d =>
                d.FindElement(By.CssSelector("h3[data-test='error']")));

            Assert.That(errorMessage.Displayed, Is.True);
            Assert.That(errorMessage.Text, Does.Contain("Username and password do not match"));
        }

        // Add To Cart Flow Test
        [Test]
        public void AddToCartFlowTest()
        {
            Login("standard_user", "secret_sauce");

            wait!.Until(d => d.Url.Contains("inventory"));

            // Add first product to cart
            driver!.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();

            // Validate cart badge shows 1
            IWebElement cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));
            Assert.That(cartBadge.Text, Is.EqualTo("1"));

            // Click cart
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();

            // Verify item exists in cart page
            IWebElement cartItem = wait!.Until(d =>
                d.FindElement(By.ClassName("inventory_item_name")));

            Assert.That(cartItem.Text, Is.EqualTo("Sauce Labs Backpack"));
        }

        [Test]
        public void CartToCheckoutStepOneTest()
        {
            Login("standard_user", "secret_sauce");

            wait.Until(d => d.Url.Contains("inventory"));

            driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();

            // Click Checkout button
            driver.FindElement(By.Id("checkout")).Click();

            wait.Until(d => d.Url.Contains("checkout-step-one"));

            Assert.That(driver.Url, Does.Contain("checkout-step-one"));
        }

        [Test]
        public void CheckoutStepOneToStepTwoTest()
        {
            Login("standard_user", "secret_sauce");

            wait.Until(d => d.Url.Contains("inventory"));

            driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            driver.FindElement(By.Id("checkout")).Click();

            // Fill checkout information
            driver.FindElement(By.Id("first-name")).SendKeys("John");
            driver.FindElement(By.Id("last-name")).SendKeys("Doe");
            driver.FindElement(By.Id("postal-code")).SendKeys("12345");

            driver.FindElement(By.Id("continue")).Click();

            wait.Until(d => d.Url.Contains("checkout-step-two"));

            Assert.That(driver.Url, Does.Contain("checkout-step-two"));
        }

        [Test]
        public void CompleteCheckoutTest()
        {
            Login("standard_user", "secret_sauce");

            wait.Until(d => d.Url.Contains("inventory"));

            driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            driver.FindElement(By.Id("checkout")).Click();

            driver.FindElement(By.Id("first-name")).SendKeys("John");
            driver.FindElement(By.Id("last-name")).SendKeys("Doe");
            driver.FindElement(By.Id("postal-code")).SendKeys("12345");
            driver.FindElement(By.Id("continue")).Click();

            driver.FindElement(By.Id("finish")).Click();

            wait.Until(d => d.Url.Contains("checkout-complete"));

            IWebElement completeHeader = driver.FindElement(By.ClassName("complete-header"));
            Assert.That(completeHeader.Text, Is.EqualTo("Thank you for your order!"));
        }

        [Test]
        public void BackToHomeAfterCheckoutTest()
        {
            Login("standard_user", "secret_sauce");

            wait.Until(d => d.Url.Contains("inventory"));

            driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            driver.FindElement(By.Id("checkout")).Click();

            driver.FindElement(By.Id("first-name")).SendKeys("John");
            driver.FindElement(By.Id("last-name")).SendKeys("Doe");
            driver.FindElement(By.Id("postal-code")).SendKeys("12345");
            driver.FindElement(By.Id("continue")).Click();
            driver.FindElement(By.Id("finish")).Click();

            // Click Back Home button
            driver.FindElement(By.Id("back-to-products")).Click();

            wait.Until(d => d.Url.Contains("inventory"));

            Assert.That(driver.Url, Does.Contain("inventory"));
        }

        [Test]
        public void CompleteEndToEndCheckoutFlowTest()
        {
            // 1️⃣ Login
            driver.FindElement(By.Id("user-name")).SendKeys("standard_user");
            driver.FindElement(By.Id("password")).SendKeys("secret_sauce");
            driver.FindElement(By.Id("login-button")).Click();

            wait.Until(d => d.Url.Contains("inventory"));
            Assert.That(driver.Url, Does.Contain("inventory"));

            // 2️⃣ Add Product to Cart
            driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();

            IWebElement cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));
            Assert.That(cartBadge.Text, Is.EqualTo("1"));

            // 3️⃣ Go to Cart
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            wait.Until(d => d.Url.Contains("cart"));
            Assert.That(driver.Url, Does.Contain("cart"));

            // 4️⃣ Click Checkout
            driver.FindElement(By.Id("checkout")).Click();
            wait.Until(d => d.Url.Contains("checkout-step-one"));

            // 5️⃣ Fill Checkout Information
            driver.FindElement(By.Id("first-name")).SendKeys("John");
            driver.FindElement(By.Id("last-name")).SendKeys("Doe");
            driver.FindElement(By.Id("postal-code")).SendKeys("12345");
            driver.FindElement(By.Id("continue")).Click();

            wait.Until(d => d.Url.Contains("checkout-step-two"));
            Assert.That(driver.Url, Does.Contain("checkout-step-two"));

            // 6️⃣ Finish Order
            driver.FindElement(By.Id("finish")).Click();
            wait.Until(d => d.Url.Contains("checkout-complete"));

            // 7️⃣ Validate Success Message
            IWebElement successMessage = driver.FindElement(By.ClassName("complete-header"));
            Assert.That(successMessage.Text, Is.EqualTo("Thank you for your order!"));

            // 8️⃣ Back to Home
            driver.FindElement(By.Id("back-to-products")).Click();
            wait.Until(d => d.Url.Contains("inventory"));

            // 9️⃣ Final Validation
            Assert.That(driver.Url, Does.Contain("inventory"));
        }


        // 🔹 Reusable Login Method
        private void Login(string username, string password)
        {
            driver!.FindElement(By.Id("user-name")).Clear();
            driver.FindElement(By.Id("user-name")).SendKeys(username);

            driver.FindElement(By.Id("password")).Clear();
            driver.FindElement(By.Id("password")).SendKeys(password);

            driver.FindElement(By.Id("login-button")).Click();
        }
        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }
    }
}
