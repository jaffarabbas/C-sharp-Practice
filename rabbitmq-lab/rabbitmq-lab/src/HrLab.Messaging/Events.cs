using System.Text.Json;

namespace HrLab.Messaging;

/// <summary>
/// An EVENT: something that already happened, named in the past tense.
/// The publisher does not know (or care) who listens to it.
/// </summary>
public sealed record LeaveApproved(
    Guid LeaveId,
    int EmployeeId,
    string EmployeeName,
    DateOnly FromDate,
    DateOnly ToDate,
    string ApprovedBy,
    DateTimeOffset ApprovedAt)
{
    public int Days => ToDate.DayNumber - FromDate.DayNumber + 1;
}

/// <summary>Metadata about the delivery, separate from the business data.</summary>
public sealed record MessageContext(
    string? MessageId,
    string? CorrelationId,
    string RoutingKey,
    int Attempt,
    bool Redelivered);

public static class Json
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
