using RabbitMQ.Client;

namespace HrLab.Messaging;

/// <summary>
/// "Topology" = the exchanges, queues and bindings inside RabbitMQ.
///
///   API ──publish "leave.approved"──▶ [hr.events] (topic exchange)
///                                        │  binding "leave.approved"
///                    ┌───────────────────┴───────────────────┐
///                    ▼                                       ▼
///      notifications.leave-approved             payroll.leave-approved
///            │ nack ▲ after TTL                        │ nack ▲ after TTL
///            ▼      │                                  ▼      │
///      notifications.leave-approved.retry       payroll.leave-approved.retry
///      notifications.leave-approved.dead        payroll.leave-approved.dead
///
/// Every consumer gets its OWN queue, so each one receives its own copy of
/// every event (publish/subscribe). Several instances of the SAME worker share
/// one queue and split the work (competing consumers).
/// </summary>
public static class Topology
{
    public const string EventsExchange = "hr.events";

    public static class RoutingKeys
    {
        public const string LeaveApproved = "leave.approved";
    }

    public static string RetryQueue(string queue) => $"{queue}.retry";
    public static string DeadQueue(string queue) => $"{queue}.dead";

    /// <summary>Declaring is idempotent: safe to call on every startup.</summary>
    public static Task DeclareExchangesAsync(IChannel channel, CancellationToken ct = default) =>
        channel.ExchangeDeclareAsync(
            exchange: EventsExchange,
            type: ExchangeType.Topic,
            durable: true,          // survives a broker restart
            autoDelete: false,
            cancellationToken: ct);

    public static async Task DeclareConsumerQueueAsync(
        IChannel channel,
        string queue,
        IEnumerable<string> bindingKeys,
        int retryDelayMs,
        CancellationToken ct = default)
    {
        await DeclareExchangesAsync(channel, ct);

        // 1. Main queue. A message rejected with requeue:false is "dead-lettered".
        //    "" is the default exchange, which routes straight to the queue whose
        //    name equals the routing key, so it lands in OUR retry queue only
        //    (payroll is not affected when notifications fails).
        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = RetryQueue(queue),
            },
            cancellationToken: ct);

        // 2. Retry queue. Nobody consumes it. Messages sit here for retryDelayMs,
        //    expire, and are dead-lettered BACK into the main queue. That is a
        //    delayed retry with zero extra code or plugins.
        await channel.QueueDeclareAsync(
            queue: RetryQueue(queue),
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = retryDelayMs,
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = queue,
            },
            cancellationToken: ct);

        // 3. Dead (parking) queue. Messages that failed MaxAttempts times wait
        //    here for a human. Nothing is lost.
        await channel.QueueDeclareAsync(
            queue: DeadQueue(queue),
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        // 4. Subscribe the main queue to the events it cares about.
        //    Topic patterns: "*" = exactly one word, "#" = zero or more words.
        foreach (var key in bindingKeys)
        {
            await channel.QueueBindAsync(queue, EventsExchange, key, cancellationToken: ct);
        }
    }
}
