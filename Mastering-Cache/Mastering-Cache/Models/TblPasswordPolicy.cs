using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class TblPasswordPolicy
{
    public long PasswordPolicyId { get; set; }

    public long CompanyId { get; set; }

    public int MinimumLength { get; set; }

    public int MaximumLength { get; set; }

    public bool RequireUppercase { get; set; }

    public bool RequireLowercase { get; set; }

    public bool RequireDigit { get; set; }

    public bool RequireSpecialCharacter { get; set; }

    public int MinimumUniqueCharacters { get; set; }

    public bool ProhibitCommonPasswords { get; set; }

    public bool ProhibitSequentialCharacters { get; set; }

    public bool ProhibitRepeatingCharacters { get; set; }

    public int? PasswordExpirationDays { get; set; }

    public int? PasswordHistoryCount { get; set; }

    public bool EnablePasswordExpiry { get; set; }

    public int? MaxLoginAttempts { get; set; }

    public int? LockoutDurationMinutes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public string? Description { get; set; }

    public virtual TblCompany Company { get; set; } = null!;
}
