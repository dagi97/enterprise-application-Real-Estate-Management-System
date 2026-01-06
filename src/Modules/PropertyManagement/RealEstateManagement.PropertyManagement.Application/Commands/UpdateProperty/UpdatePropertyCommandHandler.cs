using MediatR;
using RealEstate.Property.Application.Interfaces;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Shared.Domain.ValueObjects;

namespace RealEstate.Property.Application.Commands.UpdateProperty;

public sealed class UpdatePropertyCommandHandler
    : IRequestHandler<UpdatePropertyCommand>
{
    private readonly IPropertyRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public UpdatePropertyCommandHandler(
        IPropertyRepository repository,
        IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(UpdatePropertyCommand request, CancellationToken ct)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, ct);

        if (property is null)
            throw new InvalidOperationException($"Property with ID {request.PropertyId} not found");

        Address? address = null;
        if (request.Street != null && request.City != null && request.SubCity != null && request.ZipCode != null)
        {
            address = new Address(request.Street, request.City, request.SubCity, request.ZipCode);
        }

        Money? price = null;
        if (request.PriceAmount.HasValue && request.Currency != null)
        {
            price = new Money(request.PriceAmount.Value, request.Currency);
        }

        PropertyFeatures? features = null;
        if (request.SizeSqMeters.HasValue && request.Bedrooms.HasValue && 
            request.Bathrooms.HasValue && request.YearBuilt.HasValue)
        {
            features = new PropertyFeatures(
                request.SizeSqMeters.Value,
                request.Bedrooms.Value,
                request.Bathrooms.Value,
                request.YearBuilt.Value);
        }

        property.UpdateDetails(address, price, features);

        // Save to database (includes saving events to Outbox)
        await _repository.UpdateAsync(property, ct);
        
        // Dispatch events for in-process handling via MediatR
        await _eventDispatcher.DispatchDomainEventsAsync(property, ct);
        
        // Clear domain events after publishing
        property.ClearDomainEvents();
    }
}
