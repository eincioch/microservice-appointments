using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microservice.Appointments.Application.EventBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Microservice.Appointments.Infrastructure.EventBus;

/// <summary>
/// Event bus implementation using RabbitMQ as the underlying messaging infrastructure.
/// </summary>
public sealed class RabbitMqEventBus(IConnection connection, IModel channel, string exchangeName) : IEventBus
{
    private readonly IConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    private readonly IModel _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    private readonly string _exchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
    private bool _disposed;

    private const string DockerRabbitMqErrorMessage = "Unable to connect to RabbitMQ. Make sure Docker is running and the RabbitMQ container is properly configured.";
    private const string HostUnreachableErrorMessage = "Unable to connect to RabbitMQ. Please ensure the RabbitMQ host is reachable. Check if Docker and RabbitMQ are running.";

    /// <summary>
    /// Asynchronously creates and initializes a new instance of <see cref="RabbitMqEventBus"/>.
    /// </summary>
    /// <param name="hostName">The RabbitMQ host name.</param>
    /// <param name="exchangeName">The name of the Exchange to use for publishing events.</param>
    /// <param name="queueName">The name of the Queue</param>
    /// <returns>A newly initialized <see cref="RabbitMqEventBus"/>.</returns>

    public static Task<RabbitMqEventBus> CreateAsync(string hostName, string exchangeName, string queueName)
    {
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ArgumentNullException(nameof(hostName));

        if (string.IsNullOrWhiteSpace(exchangeName))
            throw new ArgumentNullException(nameof(exchangeName));

        try
        {
            var factory = new ConnectionFactory { HostName = hostName };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty);

            return Task.FromResult(new RabbitMqEventBus(connection, channel, exchangeName));
        }
        catch (BrokerUnreachableException ex)
        {
            throw new InvalidOperationException(DockerRabbitMqErrorMessage, ex);
        }
        catch (SocketException ex)
        {
            throw new InvalidOperationException(HostUnreachableErrorMessage, ex);
        }
    }

    public Task PublishAsync<T>(T @event, string routingKey)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventBus));

        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        if (string.IsNullOrWhiteSpace(routingKey))
            throw new ArgumentNullException(nameof(routingKey));

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    public async Task SubscribeAsync<T>(string routingKey, Func<T, Task> handler)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventBus));

        if (string.IsNullOrWhiteSpace(routingKey))
            throw new ArgumentNullException(nameof(routingKey));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        await Task.Run(() =>
        {
            _channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: routingKey, exchange: _exchangeName, routingKey: routingKey);
        });

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, eventArgs) =>
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var deserializedEvent = JsonSerializer.Deserialize<T>(messageBody);

                if (deserializedEvent != null)
                {
                    await handler(deserializedEvent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando el mensaje: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: routingKey, autoAck: true, consumer: consumer);
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        _disposed = true;

        if (_channel.IsOpen)
            _channel.Close();

        _channel.Dispose();

        if (_connection.IsOpen)
            _connection.Close();

        _connection.Dispose();

        return ValueTask.CompletedTask;
    }
}