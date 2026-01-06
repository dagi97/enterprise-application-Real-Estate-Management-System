using MediatR;

namespace RealEstate.Property.Application.Commands.RegisterProperty;

public sealed record RegisterPropertyCommand(
    string City,
    string SubCity,
    string Street,
    string ZipCode,
    decimal PriceAmount,
    string Currency
) : IRequest<Guid>;  

