using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertyReserved(
    PropertyId PropertyId,
    DateTime OccurredOn
) : IDomainEvent;