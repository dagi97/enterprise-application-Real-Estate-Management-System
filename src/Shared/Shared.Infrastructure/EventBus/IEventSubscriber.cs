using Shared.Domain.Events;

namespace Shared.Infrastructure.EventBus;

public interface IEventSubscriber<in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

