using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblCompany
{
    public long CompanyId { get; set; }

    public string CompanyRefNo { get; set; } = null!;

    public string CompanyTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
