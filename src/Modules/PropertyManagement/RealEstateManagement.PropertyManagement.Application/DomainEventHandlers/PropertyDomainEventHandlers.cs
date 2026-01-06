using MediatR;
using RealEstate.Property.Application.Notifications;

namespace RealEstate.Property.Application.DomainEventHandlers
{
     
    
    public sealed class PropertyRegisteredHandler : INotificationHandler<PropertyRegisteredNotification>
    {
        public Task Handle(PropertyRegisteredNotification notification, CancellationToken cancellationToken)
        {
             Console.WriteLine($"[In-Process Event] Property registered: {notification.DomainEvent.PropertyId}");
            return Task.CompletedTask;
        }
    }

    public sealed class PropertyReservedHandler : INotificationHandler<PropertyReservedNotification>
    {
        public Task Handle(PropertyReservedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[In-Process Event] Property reserved: {notification.DomainEvent.PropertyId.Id}");
            return Task.CompletedTask;
        }
    }

    public sealed class PropertySoldHandler : INotificationHandler<PropertySoldNotification>
    {
        public Task Handle(PropertySoldNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[In-Process Event] Property sold: {notification.DomainEvent.PropertyId.Id}");
            return Task.CompletedTask;
        }
    }

    public sealed class PropertyUpdatedHandler : INotificationHandler<PropertyUpdatedNotification>
    {
        public Task Handle(PropertyUpdatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[In-Process Event] Property updated: {notification.DomainEvent.PropertyId}");
            return Task.CompletedTask;
        }
    }

    public sealed class PropertyReservationCancelledHandler : INotificationHandler<PropertyReservationCancelledNotification>
    {
        public Task Handle(PropertyReservationCancelledNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[In-Process Event] Property reservation cancelled: {notification.DomainEvent.PropertyId.Id}");
            return Task.CompletedTask;
        }
    }

    public sealed class PropertyWithdrawnHandler : INotificationHandler<PropertyWithdrawnNotification>
    {
        public Task Handle(PropertyWithdrawnNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[In-Process Event] Property withdrawn: {notification.DomainEvent.PropertyId.Id}");
            return Task.CompletedTask;
        }
    }
}
