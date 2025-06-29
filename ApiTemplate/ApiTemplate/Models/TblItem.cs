using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblItem
{
    public int TranId { get; set; }

    public string? ItemRefNo { get; set; }

    public string? ItemTitle { get; set; }

    public double? SaleRate { get; set; }

    public DateTime TransactionDate { get; set; }
}
