using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HrLab.Messaging;

/// <summary>
/// Register as a SINGLETON. Keeps one connection + one publishing channel open.
///
/// Publisher confirms are ON: BasicPublishAsync only completes after the broker
/// says "I have stored it". If the broker refuses it, or (mandatory:true) no
/// queue is bound for the routing key, it throws a PublishException instead of
/// silently dropping your message.
/// </summary>
public sealed class RabbitPublisher(IOptions<RabbitOptions> options, ILogger<RabbitPublisher> logger)
    : IAsyncDisposable
{
    private readonly RabbitOptions _options = options.Value;

    // A channel must not be used for publishing from several threads at once,
    // and ASP.NET handles requests in parallel, so we serialize access.
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task<string> PublishAsync<T>(
        string routingKey, T message, string? correlationId = null, CancellationToken ct = default)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message, Json.Options);
        var props = new BasicProperties
        {
            Persistent = true,                          // written to disk, survives broker restart
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),      // used by consumers for idempotency
            CorrelationId = correlationId ?? Guid.NewGuid().ToString(),
            Type = typeof(T).Name,
            AppId = "hr-api",
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
        };

        await _lock.WaitAsync(ct);
        try
        {
            var channel = await GetChannelAsync(ct);
            await channel.BasicPublishAsync(
                exchange: Topology.EventsExchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body,
                cancellationToken: ct);
        }
        finally
        {
            _lock.Release();
        }

        logger.LogInformation("Published {Type} {MessageId} with key '{RoutingKey}' (confirmed by broker)",
            props.Type, props.MessageId, routingKey);
        return props.MessageId;
    }

    /// <summary>
    /// Moves parked messages from "{queue}.dead" back into "{queue}" after you
    /// have fixed the bug. Publish first, ack second: if we crash in between,
    /// the message exists twice rather than zero times (at-least-once).
    /// </summary>
    public async Task<int> RedriveAsync(string queue, int max, CancellationToken ct = default)
    {
        var moved = 0;
        await _lock.WaitAsync(ct);
        try
        {
            var channel = await GetChannelAsync(ct);
            while (moved < max)
            {
                var result = await channel.BasicGetAsync(Topology.DeadQueue(queue), autoAck: false, ct);
                if (result is null) break;   // queue is empty

                var props = new BasicProperties(result.BasicProperties) { Persistent = true };
                if (props.Headers is not null)
                {
                    var headers = new Dictionary<string, object?>(props.Headers);
                    headers.Remove("x-death");            // reset the attempt counter
                    headers.Remove("x-error");
                    props.Headers = headers;
                }

                await channel.BasicPublishAsync("", queue, mandatory: true, props, result.Body, ct);
                await channel.BasicAckAsync(result.DeliveryTag, multiple: false, ct);
                moved++;
            }
        }
        finally
        {
            _lock.Release();
        }

        logger.LogInformation("Re-drove {Count} message(s) from {Dead} to {Queue}",
            moved, Topology.DeadQueue(queue), queue);
        return moved;
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct)
    {
        if (_channel is { IsOpen: true }) return _channel;

        _connection ??= await RabbitConnector.ConnectAsync(_options, "hr-api", logger, ct);
        _channel = await _connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            ct);

        await Topology.DeclareExchangesAsync(_channel, ct);
        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
    }
}
