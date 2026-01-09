using PricingValuation.Domain.Aggregates;

namespace PricingValuation.Domain.Repositories;

public interface IPriceEstimateRepository
{
    Task<PriceEstimate?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<PriceEstimate?> GetLatestByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task AddAsync(PriceEstimate estimate, CancellationToken cancellationToken = default);
    Task UpdateAsync(PriceEstimate estimate, CancellationToken cancellationToken = default);
}

