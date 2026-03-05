using OpenQA.Selenium;

namespace TestProject1.Locators
{
    public static class CartPageLocators
    {
        public static By CheckoutButton => By.Id("checkout");
        public static By CartItemNames => By.ClassName("inventory_item_name");
        public static By CartBadge => By.ClassName("shopping_cart_badge");
    }

    public static class LoginPageLocators
    {
        public static By UsernameField => By.Id("user-name");
        public static By PasswordField => By.Id("password");
        public static By LoginButton => By.Id("login-button");
        public static By ErrorMessage => By.CssSelector("h3[data-test='error']");
    }

    public static class InventoryPageLocators
    {
        public static By BackpackAddButton => By.Id("add-to-cart-sauce-labs-backpack");
        public static By CartLink => By.ClassName("shopping_cart_link");
    }

    public static class CheckoutPageLocators
    {
        public static By FirstNameField => By.Id("first-name");
        public static By LastNameField => By.Id("last-name");
        public static By PostalCodeField => By.Id("postal-code");
        public static By ContinueButton => By.Id("continue");
        public static By FinishButton => By.Id("finish");
        public static By CompleteHeader => By.ClassName("complete-header");
        public static By BackToProductsButton => By.Id("back-to-products");
    }
}
