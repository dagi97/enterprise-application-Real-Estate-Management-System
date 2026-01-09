using MediatR;
using PropertyManagement.Application.Commands;
using PropertyManagement.Domain.Repositories;
using Shared.Domain.Events;
using Shared.Domain.EventHandlers;

namespace PropertyManagement.Application.EventHandlers;

public class PriceSuggestedEventHandler : IEventSubscriber<PriceSuggestedEvent>
{
    private readonly IPropertyRepository _propertyRepository;

    public PriceSuggestedEventHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task HandleAsync(PriceSuggestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(domainEvent.PropertyId, cancellationToken);
        if (property == null)
            return;

        property.SetSuggestedPrice(domainEvent.SuggestedPrice);
        await _propertyRepository.UpdateAsync(property, cancellationToken);
    }
}

