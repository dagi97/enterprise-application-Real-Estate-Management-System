using MediatR;
using RealEstate.Property.Application.Interfaces;

namespace RealEstate.Property.Application.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand>
{
    private readonly IPropertyRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public CancelReservationCommandHandler(
        IPropertyRepository repository,
        IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property is null)
            throw new InvalidOperationException($"Property with ID {request.PropertyId} not found");

        property.CancelReservation();

         await _repository.UpdateAsync(property, cancellationToken);
        
         await _eventDispatcher.DispatchDomainEventsAsync(property, cancellationToken);
        
         property.ClearDomainEvents();
    }
}

