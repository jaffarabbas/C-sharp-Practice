using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblCompanyUnit
{
    public long CompanyUnitId { get; set; }

    public string CompanyUnitRefNo { get; set; } = null!;

    public string CompanyUnitTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
