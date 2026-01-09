using Shared.Domain.Events;

namespace PropertyManagement.Domain.Events;

public sealed class PropertyStatusChangedEvent : DomainEvent
{
    public Guid PropertyId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }

    public PropertyStatusChangedEvent(Guid propertyId, string oldStatus, string newStatus)
    {
        PropertyId = propertyId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

