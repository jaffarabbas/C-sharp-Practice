using Shared.Dtos;
using System;
using System.Collections.Generic;

namespace DBLayer.Dtos;

public partial class TblUserRole
{
    public int UserRoleId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime UserRoleCreatedAt { get; set; }

    public bool UserRoleIsActive { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual TblUsersDto User { get; set; } = null!;
}
