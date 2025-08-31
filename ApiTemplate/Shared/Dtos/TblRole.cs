using System;
using System.Collections.Generic;

namespace DBLayer.Dtos;

public partial class TblRole
{
    public int RoleId { get; set; }

    public string RoleTitle { get; set; } = null!;

    public DateTime RoleCreatedAt { get; set; }

    public bool RoleIsActive { get; set; }
}
