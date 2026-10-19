using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace HrLab.Messaging;

public static class RabbitConnector
{
    /// <summary>
    /// One CONNECTION per process (it is a real TCP socket, expensive).
    /// Inside it you open cheap CHANNELS for actual work.
    ///
    /// AutomaticRecovery reconnects after the broker restarts, but only once a
    /// first connection succeeded, so we retry the very first connect ourselves
    /// (RabbitMQ in Docker takes a few seconds to boot).
    /// </summary>
    public static async Task<IConnection> ConnectAsync(
        RabbitOptions options, string clientName, ILogger logger, CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.User,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            ClientProvidedName = clientName,          // shows in the Management UI → Connections
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var connection = await factory.CreateConnectionAsync(ct);
                logger.LogInformation("Connected to RabbitMQ at {Host}:{Port} as '{Client}'",
                    options.Host, options.Port, clientName);
                return connection;
            }
            catch (BrokerUnreachableException ex) when (!ct.IsCancellationRequested)
            {
                var wait = TimeSpan.FromSeconds(Math.Min(attempt * 2, 10));
                logger.LogWarning("RabbitMQ not reachable yet ({Reason}). Retrying in {Wait}s...",
                    ex.InnerException?.Message ?? ex.Message, wait.TotalSeconds);
                await Task.Delay(wait, ct);
            }
        }
    }
}
