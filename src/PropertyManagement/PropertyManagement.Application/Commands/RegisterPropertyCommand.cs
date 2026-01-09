using MediatR;

namespace PropertyManagement.Application.Commands;

public record RegisterPropertyCommand(
    Guid BranchId,
    string PropertyType,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    decimal Size,
    int Bedrooms,
    int Bathrooms,
    string Condition,
    int? YearBuilt,
    string? Description) : IRequest<Guid>;

