using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblInvoice
{
    public long TranId { get; set; }

    public string TranRefNo { get; set; } = null!;

    public long ItemId { get; set; }

    public long PricingListId { get; set; }

    public long Quantity { get; set; }

    public long SalePrice { get; set; }

    public DateTime CreationDate { get; set; }
}
