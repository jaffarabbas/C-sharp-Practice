using System;
using System.Collections.Generic;

namespace Mastering_Cache.Models;

public partial class ApplicationLog
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? LogEvent { get; set; }

    public string? UserName { get; set; }

    public string? Ipaddress { get; set; }

    public string? RequestPath { get; set; }

    public string? ActionName { get; set; }

    public string? Application { get; set; }
}
