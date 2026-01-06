using MediatR;

namespace RealEstate.Property.Application.Commands.RegisterProperty;

public sealed record RegisterPropertyCommand(
    Guid BranchId,
    string City,
    string SubCity,
    string Street,
    string ZipCode,
    decimal PriceAmount,
    string Currency,
    decimal SizeSqMeters,
    int Bedrooms,
    int Bathrooms,
    int YearBuilt
) : IRequest<Guid>;  

