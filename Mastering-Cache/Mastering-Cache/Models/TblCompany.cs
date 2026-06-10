using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class TblCompany
{
    public long CompanyId { get; set; }

    public string CompanyRefNo { get; set; } = null!;

    public string CompanyTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public virtual ICollection<TblApplicationFlag> TblApplicationFlags { get; set; } = new List<TblApplicationFlag>();

    public virtual ICollection<TblPasswordPolicy> TblPasswordPolicies { get; set; } = new List<TblPasswordPolicy>();
}
