using MediatR;
using RealEstate.Property.Application.Interfaces;

namespace RealEstate.Property.Application.Commands.WithdrawProperty;

public sealed class WithdrawPropertyCommandHandler
    : IRequestHandler<WithdrawPropertyCommand>
{
    private readonly IPropertyRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public WithdrawPropertyCommandHandler(
        IPropertyRepository repository,
        IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(WithdrawPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property is null)
            throw new InvalidOperationException($"Property with ID {request.PropertyId} not found");

        property.Withdraw();

        // Save to database (includes saving events to Outbox)
        await _repository.UpdateAsync(property, cancellationToken);
        
        // Dispatch events for in-process handling via MediatR
        await _eventDispatcher.DispatchDomainEventsAsync(property, cancellationToken);
        
        // Clear domain events after publishing
        property.ClearDomainEvents();
    }
}


