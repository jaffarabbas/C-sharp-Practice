namespace TestProject1.TestData
{
    public class LoginData
    {
        public Credential validLogin { get; set; } = null!;
        public Credential invalidLogin { get; set; } = null!;
        public Checkout checkoutInfo { get; set; } = null!;
    }

    public class Credential
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
    }

    public class Checkout
    {
        public string firstName { get; set; } = null!;
        public string lastName { get; set; } = null!;
        public string postalCode { get; set; } = null!;
    }
}
