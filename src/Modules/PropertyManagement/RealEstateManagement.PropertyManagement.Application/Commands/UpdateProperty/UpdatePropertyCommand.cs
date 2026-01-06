using MediatR;

namespace RealEstate.Property.Application.Commands.UpdateProperty;

public sealed record UpdatePropertyCommand(
    Guid PropertyId,   
    string? City,
    string? SubCity,
    string? Street,
    string? ZipCode,
    decimal? PriceAmount,
    string? Currency,
    decimal? SizeSqMeters,
    int? Bedrooms,
    int? Bathrooms,
    int? YearBuilt
) : IRequest;
