using MediatR;
using RealEstate.Property.Application.Interfaces;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Shared.Domain.ValueObjects;
using RealEstate.Property.Application.Commands.UpdateProperty;

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

        var address = new Address(
            request.Street,
            request.City,
            request.SubCity,
            request.ZipCode
        );

        var price = new Money(request.PriceAmount, request.Currency);

        property.UpdateAddress(address);
        property.UpdatePrice(price);

         await _repository.UpdateAsync(property, ct);
        
         await _eventDispatcher.DispatchDomainEventsAsync(property, ct);
        
         property.ClearDomainEvents();
    }
}
