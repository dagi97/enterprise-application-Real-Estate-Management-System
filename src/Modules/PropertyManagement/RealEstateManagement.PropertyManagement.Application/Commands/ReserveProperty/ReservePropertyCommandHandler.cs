using MediatR;
using RealEstate.Property.Application.Interfaces;
using RealEstateManagement.Property.Domain.Entities;
using RealEstateManagement.Property.Domain.ValueObjects;

namespace RealEstate.Property.Application.Commands.ReserveProperty;

public sealed class ReservePropertyCommandHandler
    : IRequestHandler<ReservePropertyCommand>
{
    private readonly IPropertyRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public ReservePropertyCommandHandler(
        IPropertyRepository repository,
        IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(ReservePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property is null)
            throw new InvalidOperationException($"Property with ID {request.PropertyId} not found");

        var owner = new Owner(request.OwnerId, request.OwnerName);
        property.Reserve(owner);

         await _repository.UpdateAsync(property, cancellationToken);
        
         await _eventDispatcher.DispatchDomainEventsAsync(property, cancellationToken);
        
         property.ClearDomainEvents();
    }
}

