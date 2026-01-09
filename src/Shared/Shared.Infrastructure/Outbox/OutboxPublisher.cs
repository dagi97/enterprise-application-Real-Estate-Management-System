using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Domain.Events;
using Shared.Infrastructure.EventBus;
using System.Text.Json;

namespace Shared.Infrastructure.Outbox;

public class OutboxPublisher
{
    private readonly OutboxDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(
        OutboxDbContext context,
        IEventBus eventBus,
        ILogger<OutboxPublisher> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task PublishPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        var pendingMessages = await _context.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(cancellationToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                var domainEvent = DeserializeEvent(message);
                if (domainEvent != null)
                {
                    await _eventBus.PublishAsync(domainEvent, cancellationToken);
                    message.ProcessedOn = DateTime.UtcNow;
                    message.Error = null;
                    _logger.LogInformation("Published outbox message {MessageId}", message.Id);
                }
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                _logger.LogError(ex, "Error publishing outbox message {MessageId}", message.Id);

                if (message.RetryCount >= 5)
                {
                    message.ProcessedOn = DateTime.UtcNow; // Mark as processed to prevent infinite retries
                    message.Error = $"Max retries exceeded: {ex.Message}";
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private DomainEvent? DeserializeEvent(OutboxMessage message)
    {
        try
        {
            var eventType = Type.GetType(message.Type);
            if (eventType == null || !typeof(DomainEvent).IsAssignableFrom(eventType))
                return null;

            return JsonSerializer.Deserialize(message.Content, eventType) as DomainEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing event type {EventType}", message.Type);
            return null;
        }
    }
}

