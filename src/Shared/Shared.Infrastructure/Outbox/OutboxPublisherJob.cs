using Microsoft.Extensions.Logging;
using Quartz;

namespace Shared.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class OutboxPublisherJob : IJob
{
    private readonly OutboxPublisher _outboxPublisher;
    private readonly ILogger<OutboxPublisherJob> _logger;

    public OutboxPublisherJob(
        OutboxPublisher outboxPublisher,
        ILogger<OutboxPublisherJob> logger)
    {
        _outboxPublisher = outboxPublisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting outbox publisher job at {Time}", DateTime.UtcNow);
            await _outboxPublisher.PublishPendingEventsAsync(context.CancellationToken);
            _logger.LogInformation("Outbox publisher job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing outbox publisher job");
            throw;
        }
    }
}

