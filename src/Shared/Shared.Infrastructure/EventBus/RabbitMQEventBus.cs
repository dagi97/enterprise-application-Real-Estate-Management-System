using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Domain.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.EventBus;

public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventBus> _logger;

    public RabbitMQEventBus(ILogger<RabbitMQEventBus> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            Port = 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var eventName = domainEvent.GetType().Name;
            var exchangeName = "realestate_events";

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

            var message = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = domainEvent.Id.ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = eventName;

            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: eventName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Event {EventName} published with ID {EventId}", eventName, domainEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventName}", domainEvent.GetType().Name);
            throw;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

