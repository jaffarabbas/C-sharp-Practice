using System;

namespace DBLayer.Models;

public partial class TblResetToken
{
    public int ResetTokenId { get; set; }

    public long UserId { get; set; }

    public string TokenType { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }

    public virtual TblUser User { get; set; } = null!;

    public virtual TblTokenType TokenTypeNavigation { get; set; } = null!;
}