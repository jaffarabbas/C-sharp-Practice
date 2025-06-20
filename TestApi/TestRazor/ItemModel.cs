namespace TestRazor
{
    public class ItemModel
    {
        public int TranId { get; set; }

        public string? ItemRefNo { get; set; }

        public string? ItemTitle { get; set; }

        public double? SaleRate { get; set; }

        public DateTime TransactionDate { get; set; }
        public string? PricingTitle { get; set; }
    }
}
