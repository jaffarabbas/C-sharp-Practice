using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblTokenType
{
    public string TokenType { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TblResetToken> TblResetTokens { get; set; } = new List<TblResetToken>();
}