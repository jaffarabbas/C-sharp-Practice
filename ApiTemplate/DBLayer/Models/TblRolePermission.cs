using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblRolePermission
{
    public int RolePermissionId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public DateTime RolePermissionCreatedAt { get; set; }

    public bool RolePermissionIsActive { get; set; }

    public virtual TblPermission Permission { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
