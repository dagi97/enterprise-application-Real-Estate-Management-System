using Shared.Domain.Events;

namespace Shared.Infrastructure.EventBus;

public interface IEventBus
{
    Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}

