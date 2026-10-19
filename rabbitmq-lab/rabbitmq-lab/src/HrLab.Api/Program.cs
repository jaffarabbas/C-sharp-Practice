using HrLab.Messaging;
using RabbitMQ.Client.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection(RabbitOptions.Section));
builder.Services.AddSingleton<RabbitPublisher>();

var app = builder.Build();

app.MapGet("/", () => Results.Text("""
    HR Lab API
      POST /leaves/{employeeId}/approve     approve one leave (employee ids divisible by 13 fail in notifications)
      POST /leaves/bulk?count=50            approve many, to watch prefetch + competing consumers
      POST /admin/redrive/{queue}           move parked messages from {queue}.dead back to {queue}
    RabbitMQ UI: http://localhost:15672  (guest / guest)
    """));

// The manager clicks "Approve". The API's only jobs: save the approval and
// announce it. Who reacts (push, payroll, audit, email...) is not its business.
app.MapPost("/leaves/{employeeId:int}/approve", async (
    int employeeId, ApproveLeaveRequest? request, RabbitPublisher publisher, CancellationToken ct) =>
{
    // In a real app: UPDATE tblLeave SET Status = 'Approved' ... happens here first.
    // DB commit + publish are two separate systems; if we crash between them the
    // event is lost. Fixing exactly that is the Outbox pattern (lesson 3).
    var from = request?.FromDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    var leave = new LeaveApproved(
        LeaveId: Guid.NewGuid(),
        EmployeeId: employeeId,
        EmployeeName: request?.EmployeeName ?? $"Employee {employeeId}",
        FromDate: from,
        ToDate: request?.ToDate ?? from.AddDays(2),
        ApprovedBy: request?.ApprovedBy ?? "manager",
        ApprovedAt: DateTimeOffset.UtcNow);

    return await PublishOrProblem(async () =>
    {
        var messageId = await publisher.PublishAsync(Topology.RoutingKeys.LeaveApproved, leave, ct: ct);
        return Results.Accepted(value: new { leave.LeaveId, messageId, leave.Days });
    });
});

app.MapPost("/leaves/bulk", async (RabbitPublisher publisher, CancellationToken ct, int count = 20) =>
{
    return await PublishOrProblem(async () =>
    {
        for (var i = 1; i <= count; i++)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(i));
            var leave = new LeaveApproved(Guid.NewGuid(), i, $"Employee {i}", from, from.AddDays(i % 3),
                "manager", DateTimeOffset.UtcNow);
            await publisher.PublishAsync(Topology.RoutingKeys.LeaveApproved, leave, ct: ct);
        }
        return Results.Accepted(value: new { published = count });
    });
});

app.MapPost("/admin/redrive/{queue}", async (string queue, RabbitPublisher publisher, CancellationToken ct, int max = 100) =>
    await PublishOrProblem(async () => Results.Ok(new { queue, moved = await publisher.RedriveAsync(queue, max, ct) })));

app.Run();

static async Task<IResult> PublishOrProblem(Func<Task<IResult>> action)
{
    try
    {
        return await action();
    }
    catch (PublishException ex) when (ex.IsReturn)
    {
        // mandatory:true and no queue is bound to this routing key
        return Results.Problem(
            "RabbitMQ accepted the event but no queue is bound for it, so it would be lost. " +
            "Start at least one worker first; it creates its queue and binding on startup.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception ex) when (ex is BrokerUnreachableException or PublishException or AlreadyClosedException)
    {
        return Results.Problem($"Could not publish to RabbitMQ: {ex.Message}",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}

record ApproveLeaveRequest(string? EmployeeName, DateOnly? FromDate, DateOnly? ToDate, string? ApprovedBy);
