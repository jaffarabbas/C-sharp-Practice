using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblDivision
{
    public long DivisionId { get; set; }

    public string DivisionRefNo { get; set; } = null!;

    public string DivisionTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
