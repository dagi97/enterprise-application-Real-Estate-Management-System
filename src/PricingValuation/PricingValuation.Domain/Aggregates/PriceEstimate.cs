using PricingValuation.Domain.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Events;

namespace PricingValuation.Domain.Aggregates;

public class PriceEstimate : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public decimal EstimatedPrice { get; private set; }
    public decimal ConfidenceScore { get; private set; }
    public PricingFactors Factors { get; private set; } = null!;
    public string ModelVersion { get; private set; } = string.Empty;
    public DateTime EstimatedAt { get; private set; }
    public bool IsApplied { get; private set; }

    private PriceEstimate() { }

    private PriceEstimate(
        Guid id,
        Guid propertyId,
        decimal estimatedPrice,
        decimal confidenceScore,
        PricingFactors factors,
        string modelVersion)
    {
        Id = id;
        PropertyId = propertyId;
        EstimatedPrice = estimatedPrice;
        ConfidenceScore = confidenceScore;
        Factors = factors;
        ModelVersion = modelVersion;
        EstimatedAt = DateTime.UtcNow;
        IsApplied = false;
    }

    public static PriceEstimate Create(
        Guid propertyId,
        decimal estimatedPrice,
        decimal confidenceScore,
        PricingFactors factors,
        string modelVersion)
    {
        var estimate = new PriceEstimate(
            Guid.NewGuid(),
            propertyId,
            estimatedPrice,
            confidenceScore,
            factors,
            modelVersion);

        estimate.RaiseDomainEvent(new PriceSuggestedEvent(
            estimate.PropertyId,
            estimate.EstimatedPrice,
            estimate.ConfidenceScore,
            estimate.ModelVersion));

        return estimate;
    }

    public void MarkAsApplied()
    {
        if (IsApplied)
            return;

        IsApplied = true;
    }
}

