using PricingValuation.Domain.DomainServices;

namespace PricingValuation.Infrastructure.DomainServices;

public class PriceEstimationService : IPriceEstimationService
{
    public Task<decimal> EstimatePriceAsync(
        decimal size,
        int bedrooms,
        int bathrooms,
        string condition,
        string city,
        string state,
        int? yearBuilt = null,
        CancellationToken cancellationToken = default)
    {
        // Simplified AI-driven price estimation
        // In a real implementation, this would call an ML model or external API
        
        var basePrice = 100000m; // Base price per sq meter
        var sizeMultiplier = size * 1500m;
        var bedroomValue = bedrooms * 50000m;
        var bathroomValue = bathrooms * 30000m;
        
        var conditionMultiplier = condition.ToLower() switch
        {
            "excellent" => 1.2m,
            "good" => 1.0m,
            "fair" => 0.8m,
            "poor" => 0.6m,
            _ => 1.0m
        };

        var locationMultiplier = GetLocationMultiplier(city, state);
        
        var yearBuiltMultiplier = 1.0m;
        if (yearBuilt.HasValue)
        {
            var age = DateTime.Now.Year - yearBuilt.Value;
            if (age < 5) yearBuiltMultiplier = 1.1m;
            else if (age < 10) yearBuiltMultiplier = 1.0m;
            else if (age < 20) yearBuiltMultiplier = 0.95m;
            else yearBuiltMultiplier = 0.9m;
        }

        var estimatedPrice = (sizeMultiplier + bedroomValue + bathroomValue) 
            * conditionMultiplier 
            * locationMultiplier 
            * yearBuiltMultiplier;

        return Task.FromResult(Math.Round(estimatedPrice, 2));
    }

    private decimal GetLocationMultiplier(string city, string state)
    {
        // Simplified - in real implementation, use market data API
        var premiumLocations = new[] { "New York", "Los Angeles", "San Francisco", "Seattle" };
        if (premiumLocations.Contains(city, StringComparer.OrdinalIgnoreCase))
            return 1.5m;
        
        return 1.0m;
    }
}

