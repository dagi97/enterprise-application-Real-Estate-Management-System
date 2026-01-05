using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Domain.Events;

public sealed record PropertyRegistered(PropertyId PropertyId, DateTime OccurredOn) : IDomainEvent;
