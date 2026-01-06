namespace RealEstate.Property.API;

public record UpdatePropertyRequest(
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
);

