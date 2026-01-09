namespace Shared.Domain.Events;

public sealed class PropertyRegisteredEvent : DomainEvent
{
    public Guid PropertyId { get; }
    public Guid BranchId { get; }
    public string PropertyType { get; }
    public decimal Size { get; }
    public int Bedrooms { get; }
    public string City { get; }
    public string State { get; }

    public PropertyRegisteredEvent(
        Guid propertyId,
        Guid branchId,
        string propertyType,
        decimal size,
        int bedrooms,
        string city,
        string state)
    {
        PropertyId = propertyId;
        BranchId = branchId;
        PropertyType = propertyType;
        Size = size;
        Bedrooms = bedrooms;
        City = city;
        State = state;
    }
}

