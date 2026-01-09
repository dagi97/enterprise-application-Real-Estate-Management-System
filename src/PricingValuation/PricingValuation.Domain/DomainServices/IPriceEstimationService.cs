using PricingValuation.Domain.ValueObjects;

namespace PricingValuation.Domain.DomainServices;

public interface IPriceEstimationService
{
    Task<decimal> EstimatePriceAsync(
        decimal size,
        int bedrooms,
        int bathrooms,
        string condition,
        string city,
        string state,
        int? yearBuilt = null,
        CancellationToken cancellationToken = default);
}

