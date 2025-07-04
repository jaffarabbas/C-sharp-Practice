using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblBranch
{
    public long BranchId { get; set; }

    public string BranchRefNo { get; set; } = null!;

    public string BranchTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
