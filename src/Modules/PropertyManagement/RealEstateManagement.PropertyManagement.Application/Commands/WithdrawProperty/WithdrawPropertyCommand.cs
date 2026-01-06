using MediatR;

namespace RealEstate.Property.Application.Commands.WithdrawProperty;

public sealed record WithdrawPropertyCommand(
    Guid PropertyId
) : IRequest;


