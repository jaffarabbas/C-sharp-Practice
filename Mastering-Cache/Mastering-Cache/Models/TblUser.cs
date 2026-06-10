using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class TblUser
{
    public long Userid { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Salt { get; set; } = null!;

    public virtual ICollection<TblRefreshToken> TblRefreshTokens { get; set; } = new List<TblRefreshToken>();

    public virtual ICollection<TblResetToken> TblResetTokens { get; set; } = new List<TblResetToken>();

    public virtual ICollection<TblUserRole> TblUserRoles { get; set; } = new List<TblUserRole>();
}
