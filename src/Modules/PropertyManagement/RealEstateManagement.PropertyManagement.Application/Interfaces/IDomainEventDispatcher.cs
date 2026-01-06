namespace RealEstate.Property.Application.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchDomainEventsAsync(object aggregate, CancellationToken cancellationToken = default);
}

