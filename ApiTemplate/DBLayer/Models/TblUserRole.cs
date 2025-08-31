using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblUserRole
{
    public int UserRoleId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime UserRoleCreatedAt { get; set; }

    public bool UserRoleIsActive { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
