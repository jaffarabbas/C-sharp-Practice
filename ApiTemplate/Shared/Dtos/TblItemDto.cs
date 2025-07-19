namespace ApiTemplate.Dtos
{
    public class TblItemDto
    {
        public int TranId { get; set; }

        public string? ItemRefNo { get; set; }

        public string? ItemTitle { get; set; }

        public double? SaleRate { get; set; }

        public DateTime TransactionDate { get; set; }
        public string? PricingTitle { get; set; }
    }
}
