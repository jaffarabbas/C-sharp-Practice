using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblResource
{
    public int ResourceId { get; set; }

    public string ResourceName { get; set; } = null!;

    public DateTime ResourceCreatedAt { get; set; }

    public bool ResourceIsActive { get; set; }

    public virtual ICollection<TblPermission> TblPermissions { get; set; } = new List<TblPermission>();
}
