using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblPricingList
{
    public int TranId { get; set; }

    public int? ItemId { get; set; }

    public string? PricingTitle { get; set; }

    public double? SaleRate { get; set; }

    public DateTime EffictiveFrom { get; set; }

    public DateTime EffictiveTo { get; set; }
}
