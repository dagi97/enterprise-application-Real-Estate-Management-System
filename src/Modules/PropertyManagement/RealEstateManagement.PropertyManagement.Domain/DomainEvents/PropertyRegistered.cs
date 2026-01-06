using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertyRegistered(
    PropertyId PropertyId,
    BranchId BranchId,
    Address Address,
    DateTime OccurredOn
) : IDomainEvent;
