using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Domain.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.EventBus;

public class RabbitMQEventSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventSubscriber> _logger;
    private readonly string _exchangeName = "realestate_events";

    public RabbitMQEventSubscriber(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQEventSubscriber> logger)
    {
        _serviceProvider = serviceProvider;
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

        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield(); // Return control to caller

        var queueName = _channel.QueueDeclare().QueueName;

        // Subscribe to all events
        _channel.QueueBind(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: "#");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                var eventType = ea.BasicProperties.Type;

                _logger.LogInformation("Received event: {EventType} with routing key: {RoutingKey}", eventType, routingKey);

                await ProcessEventAsync(eventType, message, stoppingToken);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event");
                _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue on failure
            }
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessEventAsync(string eventType, string message, CancellationToken cancellationToken)
    {
        var type = Type.GetType(eventType);
        if (type == null || !typeof(DomainEvent).IsAssignableFrom(type))
        {
            _logger.LogWarning("Unknown event type: {EventType}", eventType);
            return;
        }

        var domainEvent = JsonSerializer.Deserialize(message, type) as DomainEvent;
        if (domainEvent == null)
        {
            _logger.LogWarning("Failed to deserialize event: {EventType}", eventType);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var subscriberType = typeof(IEventSubscriber<>).MakeGenericType(type);
        var subscribers = scope.ServiceProvider.GetServices(subscriberType);

        foreach (var subscriber in subscribers)
        {
            var method = subscriberType.GetMethod(nameof(IEventSubscriber<DomainEvent>.HandleAsync));
            if (method != null)
            {
                await (Task)method.Invoke(subscriber, new object[] { domainEvent, cancellationToken })!;
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

