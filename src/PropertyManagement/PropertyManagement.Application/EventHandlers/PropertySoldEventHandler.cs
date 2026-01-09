using PropertyManagement.Domain.Repositories;
using Shared.Domain.Events;
using Shared.Domain.EventHandlers;

namespace PropertyManagement.Application.EventHandlers;

public class PropertySoldEventHandler : IEventSubscriber<PropertySoldEvent>
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertySoldEventHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task HandleAsync(PropertySoldEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(domainEvent.PropertyId, cancellationToken);
        if (property == null)
            return;

        property.MarkAsSold();
        await _propertyRepository.UpdateAsync(property, cancellationToken);
    }
}

