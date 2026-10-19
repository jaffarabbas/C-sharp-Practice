using HrLab.Messaging;
using Microsoft.Extensions.Options;

namespace HrLab.NotificationWorker;

/// <summary>
/// Sends the "your leave was approved" push notification.
/// In your real system this is where the FCM call goes, OUT of the API request,
/// so a slow or down FCM never makes the manager's Approve button hang.
/// </summary>
public sealed class LeaveApprovedNotifier(IOptions<RabbitOptions> options, ILogger<LeaveApprovedNotifier> logger)
    : QueueWorker<LeaveApproved>(options, logger)
{
    protected override string QueueName => "notifications.leave-approved";
    protected override string[] BindingKeys => [Topology.RoutingKeys.LeaveApproved];

    protected override async Task HandleAsync(LeaveApproved leave, MessageContext context, CancellationToken ct)
    {
        // Simulated outage so you can WATCH retries and the dead queue work:
        // any employee id divisible by 13 always fails.
        if (leave.EmployeeId % 13 == 0)
        {
            throw new HttpRequestException("FCM timed out (simulated for employee ids divisible by 13)");
        }

        await Task.Delay(300, ct); // pretend to call FCM

        logger.LogInformation(
            "PUSH sent to {Name} (#{EmployeeId}): leave {From} to {To} ({Days} days) approved by {By}  [attempt {Attempt}, msg {MessageId}]",
            leave.EmployeeName, leave.EmployeeId, leave.FromDate, leave.ToDate, leave.Days,
            leave.ApprovedBy, context.Attempt, context.MessageId);
    }
}
