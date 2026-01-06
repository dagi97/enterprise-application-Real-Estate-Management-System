using MediatR;
using RealEstate.Property.Application.Interfaces;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Shared.Domain.ValueObjects;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;

namespace RealEstate.Property.Application.Commands.RegisterProperty;

public sealed class RegisterPropertyCommandHandler
    : IRequestHandler<RegisterPropertyCommand, Guid>
{
    private readonly IPropertyRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public RegisterPropertyCommandHandler(
        IPropertyRepository repository,
        IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Guid> Handle(RegisterPropertyCommand request, CancellationToken cancellationToken)
    {
        // Generate unique PropertyId on the server side
        var propertyId = Guid.NewGuid();
        
        var property = new PropertyAggregate(
            new PropertyId(propertyId),
            new BranchId(request.BranchId),
            new Address(request.Street, request.City, request.SubCity, request.ZipCode),
            new Money(request.PriceAmount, request.Currency),
            new PropertyFeatures(request.SizeSqMeters, request.Bedrooms, request.Bathrooms, request.YearBuilt)
        );

        // Save to database (includes saving events to Outbox)
        await _repository.AddAsync(property, cancellationToken);
        
        // Dispatch events for in-process handling via MediatR
        await _eventDispatcher.DispatchDomainEventsAsync(property, cancellationToken);
        
        // Clear domain events after publishing
        property.ClearDomainEvents();
        
        return propertyId;
    }
}
