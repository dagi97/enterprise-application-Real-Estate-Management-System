using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertySold(
    PropertyId PropertyId,
    BranchId BranchId,
    DateTime OccurredOn
) : IDomainEvent;