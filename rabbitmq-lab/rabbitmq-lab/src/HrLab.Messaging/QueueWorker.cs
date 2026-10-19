using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace HrLab.Messaging;

/// <summary>
/// Base class for a consumer. You only write HandleAsync; this class does:
///   • connect + declare the queue topology
///   • prefetch (back-pressure)
///   • manual ack AFTER the work succeeds (at-least-once delivery)
///   • delayed retries via the .retry queue, then park in .dead
///
/// The golden rule: ack only when the work is really done. If the process
/// dies before the ack, RabbitMQ gives the message to another consumer.
/// Which means a handler CAN see the same message twice → handlers must be
/// idempotent.
/// </summary>
public abstract class QueueWorker<TMessage>(IOptions<RabbitOptions> options, ILogger logger)
    : BackgroundService
{
    private readonly RabbitOptions _options = options.Value;
    private IConnection? _connection;
    private IChannel? _consumeChannel;
    private IChannel? _publishChannel;
    private CancellationToken _stopping;

    protected abstract string QueueName { get; }
    protected abstract string[] BindingKeys { get; }
    protected abstract Task HandleAsync(TMessage message, MessageContext context, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stopping = stoppingToken;
        var clientName = $"{QueueName}@{Environment.MachineName}#{Environment.ProcessId}";
        _connection = await RabbitConnector.ConnectAsync(_options, clientName, logger, stoppingToken);

        _consumeChannel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        _publishChannel = await _connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
            stoppingToken);

        await Topology.DeclareConsumerQueueAsync(
            _consumeChannel, QueueName, BindingKeys, _options.RetryDelayMs, stoppingToken);

        // Prefetch: the broker sends at most N unacked messages to this consumer.
        // Without it, one consumer can grab the whole queue and starve the others.
        await _consumeChannel.BasicQosAsync(prefetchSize: 0, prefetchCount: _options.PrefetchCount,
            global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_consumeChannel);
        consumer.ReceivedAsync += OnReceivedAsync;

        await _consumeChannel.BasicConsumeAsync(QueueName, autoAck: false, consumer, stoppingToken);
        logger.LogInformation("Listening on '{Queue}' (bindings: {Keys}, prefetch {Prefetch}, max attempts {Max})",
            QueueName, string.Join(", ", BindingKeys), _options.PrefetchCount, _options.MaxAttempts);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // normal shutdown
        }
    }

    private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        var channel = _consumeChannel!;

        // Deliveries already buffered when the connection dropped can still reach
        // us. Their ack would fail anyway, and the broker has already requeued
        // them, so skip the work instead of doing it twice.
        if (!channel.IsOpen) return;

        var attempt = GetAttempt(ea.BasicProperties);
        var context = new MessageContext(
            ea.BasicProperties.MessageId,
            ea.BasicProperties.CorrelationId,
            ea.RoutingKey,
            attempt,
            ea.Redelivered);

        // Step 1: run YOUR code. Only its exceptions count as "the work failed".
        Exception? failure = null;
        var poison = false;
        try
        {
            var message = JsonSerializer.Deserialize<TMessage>(ea.Body.Span, Json.Options)
                          ?? throw new JsonException("Body was null");
            await HandleAsync(message, context, _stopping);
        }
        catch (JsonException ex)
        {
            // A "poison message": retrying will never fix bad JSON, so park it now.
            failure = ex;
            poison = true;
        }
        catch (Exception ex)
        {
            failure = ex;
        }

        // Step 2: tell RabbitMQ the outcome (ack / retry / park).
        try
        {
            if (failure is null)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            else if (!poison && attempt < _options.MaxAttempts)
            {
                logger.LogWarning("Attempt {Attempt}/{Max} failed for {MessageId}: {Error}. Retrying in {Delay}s",
                    attempt, _options.MaxAttempts, context.MessageId, failure.Message, _options.RetryDelayMs / 1000.0);

                // requeue:false → dead-lettered to our .retry queue (see Topology).
                // requeue:true would put it straight back at the head of the queue
                // and spin in a hot loop while FCM is still down.
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
            else
            {
                logger.LogError("Giving up on {MessageId} after {Attempt} attempt(s), parking in '{Dead}': {Error}",
                    context.MessageId, attempt, Topology.DeadQueue(QueueName), failure.Message);

                // Park FIRST (confirmed), ack SECOND. Crash in between = duplicate, never loss.
                await ParkAsync(ea, poison ? $"Invalid JSON: {failure.Message}" : failure.Message);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
        }
        catch (OperationInterruptedException ex)
        {
            // The channel died (broker restart, network) before we could ack/nack.
            // Not our handler's fault: the message is still unacked, so RabbitMQ
            // will deliver it again once we reconnect. Idempotency handles the repeat.
            logger.LogWarning("Channel closed before {MessageId} could be settled ({Reason}); it will be redelivered",
                context.MessageId, ex.Message);
        }
    }

    private async Task ParkAsync(BasicDeliverEventArgs ea, string error)
    {
        var props = new BasicProperties(ea.BasicProperties)
        {
            // Force persistent: if the original was sent as transient (some tools do
            // that by default), the parked copy would vanish on a broker restart.
            Persistent = true,
            Headers = new Dictionary<string, object?>(ea.BasicProperties.Headers ?? new Dictionary<string, object?>())
            {
                ["x-error"] = error,
                ["x-failed-at"] = DateTimeOffset.UtcNow.ToString("O"),
                ["x-original-routing-key"] = ea.RoutingKey,
            },
        };

        await _publishChannel!.BasicPublishAsync(
            exchange: "",                               // default exchange → queue by name
            routingKey: Topology.DeadQueue(QueueName),
            mandatory: true,
            basicProperties: props,
            body: ea.Body);
    }

    /// <summary>
    /// RabbitMQ adds an "x-death" header every time a message is dead-lettered.
    /// Counting how often it was rejected from OUR queue = how many attempts failed.
    /// </summary>
    private int GetAttempt(IReadOnlyBasicProperties props)
    {
        if (props.Headers?.TryGetValue("x-death", out var raw) != true || raw is not IEnumerable<object> deaths)
            return 1;

        long failures = 0;
        foreach (var entry in deaths.OfType<IDictionary<string, object?>>())
        {
            if (AsString(entry, "queue") == QueueName && AsString(entry, "reason") == "rejected"
                && entry.TryGetValue("count", out var count) && count is not null)
            {
                failures += Convert.ToInt64(count);
            }
        }

        return (int)failures + 1;
    }

    // AMQP header strings arrive as byte[]
    private static string? AsString(IDictionary<string, object?> map, string key) =>
        map.TryGetValue(key, out var v) ? v switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string s => s,
            _ => v?.ToString(),
        } : null;

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        // Any message still unacked is returned to the queue when the channel closes.
        if (_consumeChannel is not null) await _consumeChannel.CloseAsync(cancellationToken);
        if (_publishChannel is not null) await _publishChannel.CloseAsync(cancellationToken);
        if (_connection is not null) await _connection.CloseAsync(cancellationToken);
    }
}
