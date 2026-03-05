using NUnit.Framework;
using OpenQA.Selenium;
using TestProject1.Pages;

namespace TestProject1
{
    public class Tests2 : BaseTest
    {
        // Login Success Test
        [Test]
        public void LoginSuccessTest()
        {
            var loginPage = new LoginPage(driver);
            loginPage.Login(testData.validLogin.username, testData.validLogin.password);

            wait.Until(d => d.Url.Contains("inventory"));
            Assert.That(driver.Url, Does.Contain("inventory"));
        }

        // Login Fail Test
        [Test]
        public void LoginFailTest()
        {
            var loginPage = new LoginPage(driver);
            loginPage.Login(testData.invalidLogin.username, testData.invalidLogin.password);

            var errorMessage = wait.Until(d => d.FindElement(By.CssSelector("h3[data-test='error']")));
            Assert.That(errorMessage.Displayed, Is.True);
            Assert.That(errorMessage.Text, Does.Contain("Username and password do not match"));
        }

        // Add to Cart Test
        [Test]
        public void AddToCartTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);

            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            inventoryPage.AddBackpackToCart();

            var cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));
            Assert.That(cartBadge.Text, Is.EqualTo("1"));
        }

        // Cart → Checkout Step One
        [Test]
        public void CartToCheckoutStepOneTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);

            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();

            cartPage.ClickCheckout();

            wait.Until(d => d.Url.Contains("checkout-step-one"));
            Assert.That(driver.Url, Does.Contain("checkout-step-one"));
        }

        // Checkout Step One → Step Two
        [Test]
        public void CheckoutStepOneToStepTwoTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();
            cartPage.ClickCheckout();

            checkoutPage.EnterCheckoutInfo(
                testData.checkoutInfo.firstName,
                testData.checkoutInfo.lastName,
                testData.checkoutInfo.postalCode);

            wait.Until(d => d.Url.Contains("checkout-step-two"));
            Assert.That(driver.Url, Does.Contain("checkout-step-two"));
        }

        // Complete Checkout
        [Test]
        public void CompleteCheckoutTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();
            cartPage.ClickCheckout();

            checkoutPage.EnterCheckoutInfo(
                testData.checkoutInfo.firstName,
                testData.checkoutInfo.lastName,
                testData.checkoutInfo.postalCode);

            checkoutPage.FinishOrder();

            wait.Until(d => d.Url.Contains("checkout-complete"));

            Assert.That(checkoutPage.GetSuccessMessage(), Is.EqualTo("Thank you for your order!"));
        }

        // Back to Home After Checkout
        [Test]
        public void BackToHomeAfterCheckoutTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();
            cartPage.ClickCheckout();

            checkoutPage.EnterCheckoutInfo(
                testData.checkoutInfo.firstName,
                testData.checkoutInfo.lastName,
                testData.checkoutInfo.postalCode);

            checkoutPage.FinishOrder();
            wait.Until(d => d.Url.Contains("checkout-complete"));

            checkoutPage.BackToHome();

            Assert.That(driver.Url, Does.Contain("inventory"));
        }

        // Complete End-to-End Checkout Flow
        [Test]
        public void CompleteEndToEndCheckoutFlowTest()
        {
            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            //Login
            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            //Add to Cart & Open Cart
            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();
            cartPage.ClickCheckout();

            //Checkout Step One
            checkoutPage.EnterCheckoutInfo(
                testData.checkoutInfo.firstName,
                testData.checkoutInfo.lastName,
                testData.checkoutInfo.postalCode);

            //Finish Order
            checkoutPage.FinishOrder();
            wait.Until(d => d.Url.Contains("checkout-complete"));

            Assert.That(checkoutPage.GetSuccessMessage(), Is.EqualTo("Thank you for your order!"));

            //Back to Home
            checkoutPage.BackToHome();
            Assert.That(driver.Url, Does.Contain("inventory"));
        }
    }
}
