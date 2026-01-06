using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstate.Property.Infrastructure.Persistence;

namespace RealEstate.Property.Infrastructure.Messaging;

public sealed class OutboxPublisher : IOutboxPublisher
{
    private readonly PropertyDbContext _dbContext;
    private readonly ILogger<OutboxPublisher> _logger;
     

    public OutboxPublisher(
        PropertyDbContext dbContext,
        ILogger<OutboxPublisher> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task PublishPendingEventsAsync(CancellationToken cancellationToken = default)
    {
         var pendingMessages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(100) 
            .ToListAsync(cancellationToken);

        if (pendingMessages.Count == 0)
            return;

        _logger.LogInformation("Processing {Count} pending outbox messages", pendingMessages.Count);

        foreach (var message in pendingMessages)
        {
            try
            {
                 
                _logger.LogInformation(
                    "Publishing event: {EventType}, Id: {EventId}, OccurredOn: {OccurredOn}",
                    message.Type, message.Id, message.OccurredOn);

                
                message.ProcessedOn = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish outbox message {MessageId} of type {EventType}",
                    message.Id, message.Type);
                
                
            }
        }

         
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

