using System.Collections.Concurrent;
using HrLab.Messaging;
using Microsoft.Extensions.Options;

namespace HrLab.PayrollWorker;

/// <summary>
/// Deducts approved leave from the employee's leave balance.
///
/// This handler MUST be idempotent: RabbitMQ guarantees at-least-once delivery,
/// so the same event can arrive twice (worker crashed before ack, network blip,
/// re-drive). Deducting 3 days twice would be a real payroll bug.
/// </summary>
public sealed class LeaveApprovedPayrollHandler(IOptions<RabbitOptions> options, ILogger<LeaveApprovedPayrollHandler> logger)
    : QueueWorker<LeaveApproved>(options, logger)
{
    // LAB ONLY: in-memory, so it is forgotten when the process restarts.
    // Production: a table with a UNIQUE key on LeaveId (or MessageId), written in
    // the SAME transaction as the balance update. That is lesson 4.
    private static readonly ConcurrentDictionary<Guid, byte> Processed = new();

    protected override string QueueName => "payroll.leave-approved";
    protected override string[] BindingKeys => [Topology.RoutingKeys.LeaveApproved];

    protected override async Task HandleAsync(LeaveApproved leave, MessageContext context, CancellationToken ct)
    {
        // CLAIM first, atomically. "Check, then work, then mark" has a race: two
        // copies of the same message (e.g. a redelivery after a broker restart)
        // can both pass the check and BOTH deduct. The lab caught exactly that.
        // TryAdd is atomic: only one copy wins. (DB version: INSERT into a table
        // with a UNIQUE key, in the same transaction as the balance update.)
        if (!Processed.TryAdd(leave.LeaveId, 0))
        {
            logger.LogWarning("Duplicate: leave {LeaveId} already deducted or in progress, skipping (redelivered={Redelivered})",
                leave.LeaveId, context.Redelivered);
            return; // returning normally = ack, so the duplicate is removed from the queue
        }

        try
        {
            logger.LogInformation("Deducting {Days} day(s) from {Name} (#{EmployeeId})...",
                leave.Days, leave.EmployeeName, leave.EmployeeId);

            await Task.Delay(1500, ct); // slow on purpose: gives you time to kill the worker mid-way
        }
        catch
        {
            // The work failed: release the claim so the retry is not skipped as a "duplicate".
            // (DB version: the transaction rolls back, and the INSERT disappears with it.)
            Processed.TryRemove(leave.LeaveId, out _);
            throw;
        }

        logger.LogInformation("Balance updated for #{EmployeeId}, leave {LeaveId}  [attempt {Attempt}]",
            leave.EmployeeId, leave.LeaveId, context.Attempt);
    }
}
