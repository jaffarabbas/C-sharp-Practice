using NUnit.Framework;
using OpenQA.Selenium;
using TestProject1.Pages;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using TestProject1.Utilities;

namespace TestProject1
{
    [AllureNUnit]
    [AllureSuite("Sauce Demo Test Suite")]
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
        [AllureTag("Checkout", "Smoke")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.normal)]
        [AllureDescription("Verify that a user can navigate from cart to checkout step one")]
        public void CartToCheckoutStepOneTest()
        {
            AllureReportHelper.LogStep("Starting Cart to Checkout Step One Test");

            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);

            AllureReportHelper.LogStep("Performing login");
            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            AllureReportHelper.LogStep("Adding item to cart");
            inventoryPage.AddBackpackToCart();

            AllureReportHelper.LogStep("Opening cart");
            inventoryPage.OpenCart();

            AllureReportHelper.LogStep("Proceeding to checkout");
            cartPage.ClickCheckout();

            wait.Until(d => d.Url.Contains("checkout-step-one"));
            AllureReportHelper.AttachScreenshot(driver, "Checkout Step One Page");
            AllureReportHelper.LogStep("Successfully reached checkout step one");

            Assert.That(driver.Url, Does.Contain("checkout-step-one"));
        }

        // Checkout Step One → Step Two
        [Test]
        [AllureTag("Checkout", "Smoke")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.normal)]
        [AllureDescription("Verify that a user can navigate from checkout step one to step two by providing information")]
        public void CheckoutStepOneToStepTwoTest()
        {
            AllureReportHelper.LogStep("Starting Checkout Step One to Step Two Test");

            var loginPage = new LoginPage(driver);
            var inventoryPage = new InventoryPage(driver);
            var cartPage = new CartPage(driver);
            var checkoutPage = new CheckoutPage(driver);

            AllureReportHelper.LogStep("Performing login");
            loginPage.Login(testData.validLogin.username, testData.validLogin.password);
            wait.Until(d => d.Url.Contains("inventory"));

            AllureReportHelper.LogStep("Adding item to cart and proceeding to checkout");
            inventoryPage.AddBackpackToCart();
            inventoryPage.OpenCart();
            cartPage.ClickCheckout();

            AllureReportHelper.LogStep("Entering checkout information");
            AllureReportHelper.AddParameter("First Name", testData.checkoutInfo.firstName);
            AllureReportHelper.AddParameter("Last Name", testData.checkoutInfo.lastName);
            AllureReportHelper.AddParameter("Postal Code", testData.checkoutInfo.postalCode);

            checkoutPage.EnterCheckoutInfo(
                testData.checkoutInfo.firstName,
                testData.checkoutInfo.lastName,
                testData.checkoutInfo.postalCode);

            wait.Until(d => d.Url.Contains("checkout-step-two"));
            AllureReportHelper.AttachScreenshot(driver, "Checkout Step Two Page");
            AllureReportHelper.LogStep("Successfully reached checkout step two");

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
