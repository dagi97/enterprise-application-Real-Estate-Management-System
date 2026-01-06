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
         var propertyId = Guid.NewGuid();
        
        var property = new PropertyAggregate(
            new PropertyId(propertyId),
            new Address(request.Street, request.City, request.SubCity, request.ZipCode),
            new Money(request.PriceAmount, request.Currency)
        );

         await _repository.AddAsync(property, cancellationToken);
        
         await _eventDispatcher.DispatchDomainEventsAsync(property, cancellationToken);
        
         property.ClearDomainEvents();
        
        return propertyId;
    }
}
