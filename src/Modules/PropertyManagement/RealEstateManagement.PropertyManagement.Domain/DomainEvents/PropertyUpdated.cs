using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Shared.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertyUpdated(
    PropertyId PropertyId,
    BranchId BranchId,
    Address? OldAddress,
    Address? NewAddress,
    Money? OldPrice,
    Money? NewPrice,
    DateTime OccurredOn
) : IDomainEvent;

