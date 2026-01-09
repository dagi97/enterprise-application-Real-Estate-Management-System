using MediatR;
using PricingValuation.Application.Commands;
using PricingValuation.Domain.DomainServices;
using PricingValuation.Domain.Repositories;
using PricingValuation.Domain.ValueObjects;

namespace PricingValuation.Application.Commands;

public class EstimatePriceCommandHandler : IRequestHandler<EstimatePriceCommand, decimal>
{
    private readonly IPriceEstimationService _priceEstimationService;
    private readonly IPriceEstimateRepository _priceEstimateRepository;

    public EstimatePriceCommandHandler(
        IPriceEstimationService priceEstimationService,
        IPriceEstimateRepository priceEstimateRepository)
    {
        _priceEstimationService = priceEstimationService;
        _priceEstimateRepository = priceEstimateRepository;
    }

    public async Task<decimal> Handle(EstimatePriceCommand request, CancellationToken cancellationToken)
    {
        var estimatedPrice = await _priceEstimationService.EstimatePriceAsync(
            request.Size,
            request.Bedrooms,
            request.Bathrooms,
            request.Condition,
            request.City,
            request.State,
            request.YearBuilt,
            cancellationToken);

        // Calculate confidence score (simplified)
        var confidenceScore = CalculateConfidenceScore(request);
        
        var factors = new PricingFactors(
            sizeFactor: request.Size * 100,
            locationFactor: GetLocationFactor(request.City, request.State),
            conditionFactor: GetConditionFactor(request.Condition),
            bedroomFactor: request.Bedrooms * 50000,
            yearBuiltFactor: request.YearBuilt.HasValue ? GetYearBuiltFactor(request.YearBuilt.Value) : 1.0m);

        var priceEstimate = PricingValuation.Domain.Aggregates.PriceEstimate.Create(
            request.PropertyId,
            estimatedPrice,
            confidenceScore,
            factors,
            "v1.0");

        await _priceEstimateRepository.AddAsync(priceEstimate, cancellationToken);

        return estimatedPrice;
    }

    private decimal CalculateConfidenceScore(EstimatePriceCommand request)
    {
        var score = 0.7m; // Base confidence
        if (request.YearBuilt.HasValue) score += 0.1m;
        if (!string.IsNullOrWhiteSpace(request.City)) score += 0.1m;
        if (request.Size > 0) score += 0.1m;
        return Math.Min(score, 1.0m);
    }

    private decimal GetLocationFactor(string city, string state)
    {
        // Simplified location factor - in real implementation, use market data
        return 1.0m;
    }

    private decimal GetConditionFactor(string condition)
    {
        return condition.ToLower() switch
        {
            "excellent" => 1.2m,
            "good" => 1.0m,
            "fair" => 0.8m,
            "poor" => 0.6m,
            _ => 1.0m
        };
    }

    private decimal GetYearBuiltFactor(int yearBuilt)
    {
        var age = DateTime.Now.Year - yearBuilt;
        if (age < 5) return 1.1m;
        if (age < 10) return 1.0m;
        if (age < 20) return 0.9m;
        return 0.8m;
    }
}

