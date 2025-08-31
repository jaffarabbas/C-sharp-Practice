using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblRole
{
    public int RoleId { get; set; }

    public string RoleTitle { get; set; } = null!;

    public DateTime RoleCreatedAt { get; set; }

    public bool RoleIsActive { get; set; }

    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();

    public virtual ICollection<TblUserRole> TblUserRoles { get; set; } = new List<TblUserRole>();

    public virtual ICollection<TblRole> ChildRoles { get; set; } = new List<TblRole>();

    public virtual ICollection<TblRole> ParentRoles { get; set; } = new List<TblRole>();
}
