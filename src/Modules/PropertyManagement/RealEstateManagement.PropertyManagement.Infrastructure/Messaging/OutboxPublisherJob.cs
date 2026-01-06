using Microsoft.Extensions.Logging;
using Quartz;
using RealEstate.Property.Infrastructure.Messaging;

namespace RealEstate.Property.Infrastructure.Messaging;

[DisallowConcurrentExecution]  
public sealed class OutboxPublisherJob : IJob
{
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly ILogger<OutboxPublisherJob> _logger;

    public OutboxPublisherJob(
        IOutboxPublisher outboxPublisher,
        ILogger<OutboxPublisherJob> logger)
    {
        _outboxPublisher = outboxPublisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("OutboxPublisherJob started at {Time}", DateTime.UtcNow);
        
        try
        {
            await _outboxPublisher.PublishPendingEventsAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing OutboxPublisherJob");
            throw;  
        }
        
        _logger.LogInformation("OutboxPublisherJob completed at {Time}", DateTime.UtcNow);
    }
}

