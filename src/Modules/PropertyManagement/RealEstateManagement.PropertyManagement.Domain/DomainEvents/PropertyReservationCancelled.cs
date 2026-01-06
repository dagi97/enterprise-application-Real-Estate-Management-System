using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertyReservationCancelled(
    PropertyId PropertyId,
    BranchId BranchId,
    DateTime OccurredOn
) : IDomainEvent;

