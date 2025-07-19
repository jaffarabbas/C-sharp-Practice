using System;
using System.Collections.Generic;

namespace DBLayer.Models;

public partial class TblUser
{
    public long Userid { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int AccountType { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Salt { get; set; } = null!;
}
