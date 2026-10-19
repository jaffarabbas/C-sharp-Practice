namespace HrLab.Messaging;

/// <summary>Bound from the "RabbitMq" section of appsettings.json.</summary>
public sealed class RabbitOptions
{
    public const string Section = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    /// <summary>How many unacknowledged messages one consumer may hold at a time.</summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>How long a failed message waits in the .retry queue before it is tried again.</summary>
    public int RetryDelayMs { get; set; } = 5000;

    /// <summary>After this many failed attempts the message is parked in the .dead queue.</summary>
    public int MaxAttempts { get; set; } = 3;
}
