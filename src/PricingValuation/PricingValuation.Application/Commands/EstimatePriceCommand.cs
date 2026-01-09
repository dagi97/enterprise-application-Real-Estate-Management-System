using MediatR;

namespace PricingValuation.Application.Commands;

public record EstimatePriceCommand(
    Guid PropertyId,
    decimal Size,
    int Bedrooms,
    int Bathrooms,
    string Condition,
    string City,
    string State,
    int? YearBuilt = null) : IRequest<decimal>;

