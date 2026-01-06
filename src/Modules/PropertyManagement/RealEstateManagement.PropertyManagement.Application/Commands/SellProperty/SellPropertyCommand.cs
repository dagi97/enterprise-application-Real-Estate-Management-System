using MediatR;

namespace RealEstate.Property.Application.Commands.SellProperty;

public sealed record SellPropertyCommand(
    Guid PropertyId
) : IRequest;

