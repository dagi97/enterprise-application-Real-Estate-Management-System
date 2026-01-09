using Shared.Domain.Events;

namespace Shared.Domain.EventHandlers;

public interface IEventSubscriber<in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

