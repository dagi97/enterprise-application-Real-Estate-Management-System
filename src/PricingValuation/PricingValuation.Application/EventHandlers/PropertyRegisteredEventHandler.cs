using MediatR;
using PricingValuation.Application.Commands;
using Shared.Domain.Events;
using Shared.Domain.EventHandlers;

namespace PricingValuation.Application.EventHandlers;

public class PropertyRegisteredEventHandler : IEventSubscriber<PropertyRegisteredEvent>
{
    private readonly IMediator _mediator;

    public PropertyRegisteredEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task HandleAsync(PropertyRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // When a property is registered, automatically estimate its price
        var command = new EstimatePriceCommand(
            domainEvent.PropertyId,
            domainEvent.Size,
            domainEvent.Bedrooms,
            0, // Bathrooms not in event - would need to fetch from property
            "Good", // Default condition
            domainEvent.City,
            domainEvent.State);

        await _mediator.Send(command, cancellationToken);
    }
}

