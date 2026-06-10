using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class TblRefreshToken
{
    public long RefreshTokenId { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public string? DeviceInfo { get; set; }

    public string? IpAddress { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsRevoked { get; set; }

    public bool IsUsed { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
