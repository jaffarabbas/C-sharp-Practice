using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class TblApplicationFlag
{
    public long FlagId { get; set; }

    public long CompanyId { get; set; }

    public string FlagName { get; set; } = null!;

    public string FlagValue { get; set; } = null!;

    public string? DataType { get; set; }

    public string? Description { get; set; }

    public string? PossibleValues { get; set; }

    public string? DefaultValue { get; set; }

    public bool ShowToUser { get; set; }

    public string? Category { get; set; }

    public bool IsActive { get; set; }

    public bool IsReadOnly { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string? ModuleNamespace { get; set; }

    public virtual TblCompany Company { get; set; } = null!;
}
