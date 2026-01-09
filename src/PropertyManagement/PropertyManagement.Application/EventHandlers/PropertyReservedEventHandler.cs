using PropertyManagement.Domain.Enums;
using PropertyManagement.Domain.Repositories;
using Shared.Domain.Events;
using Shared.Domain.EventHandlers;

namespace PropertyManagement.Application.EventHandlers;

public class PropertyReservedEventHandler : IEventSubscriber<PropertyReservedEvent>
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyReservedEventHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task HandleAsync(PropertyReservedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(domainEvent.PropertyId, cancellationToken);
        if (property == null)
            return;

        property.Reserve();
        await _propertyRepository.UpdateAsync(property, cancellationToken);
    }
}

