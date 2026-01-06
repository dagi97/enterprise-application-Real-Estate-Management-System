using MediatR;

namespace RealEstate.Property.Application.Commands.ReserveProperty;

public sealed record ReservePropertyCommand(
    Guid PropertyId,
    Guid OwnerId
) : IRequest;

