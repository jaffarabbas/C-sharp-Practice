using System;
using System.Collections.Generic;

namespace DBLayer.Dtos;

public partial class TblActionType
{
    public int ActionTypeId { get; set; }

    public string ActionTypeTitle { get; set; } = null!;

    public DateTime ActionTypeCreatedAt { get; set; }

    public bool ActionTypeIsActive { get; set; }

    public virtual ICollection<TblPermission> TblPermissions { get; set; } = new List<TblPermission>();
}
