using System;
using System.Collections.Generic;

namespace ApiTemplate.Models;

public partial class TblDepartment
{
    public long DepartmentId { get; set; }

    public string DepartmentRefNo { get; set; } = null!;

    public string DepartmentTitle { get; set; } = null!;

    public DateTime CreationDate { get; set; }
}
