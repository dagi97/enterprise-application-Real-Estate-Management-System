using MediatR;
using RealEstate.Property.Application.Interfaces;
using RealEstate.Property.Application.Notifications;
using RealEstate.Property.Domain.Events;

namespace RealEstate.Property.Infrastructure.Services;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchDomainEventsAsync(object aggregate, CancellationToken cancellationToken = default)
    {
         var domainEventsProperty = aggregate.GetType().GetProperty("DomainEvents");
        if (domainEventsProperty == null) return;

        var domainEvents = domainEventsProperty.GetValue(aggregate) as System.Collections.IEnumerable;
        if (domainEvents == null) return;

        foreach (var domainEvent in domainEvents)
        {
             var notification = CreateNotification(domainEvent);
            if (notification != null)
            {
                await _mediator.Publish(notification, cancellationToken);
            }
        }
    }

    private static INotification? CreateNotification(object domainEvent)
    {
        return domainEvent switch
        {
            PropertyRegistered evt => new PropertyRegisteredNotification(evt),
            PropertyReserved evt => new PropertyReservedNotification(evt),
            PropertySold evt => new PropertySoldNotification(evt),
            PropertyUpdated evt => new PropertyUpdatedNotification(evt),
            PropertyReservationCancelled evt => new PropertyReservationCancelledNotification(evt),
            PropertyWithdrawn evt => new PropertyWithdrawnNotification(evt),
            _ => null
        };
    }
}

