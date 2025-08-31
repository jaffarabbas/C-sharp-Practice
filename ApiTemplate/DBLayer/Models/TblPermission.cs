using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblPermission
{
    public int PermissionId { get; set; }

    public int ResourceId { get; set; }

    public int ActionTypeId { get; set; }

    public DateTime PermissionCreatedAt { get; set; }

    public bool PermissionIsActive { get; set; }

    public virtual TblActionType ActionType { get; set; } = null!;

    public virtual TblResource Resource { get; set; } = null!;

    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();
}
