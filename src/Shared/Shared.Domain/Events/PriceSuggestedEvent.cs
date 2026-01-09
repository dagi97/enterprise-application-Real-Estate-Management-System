namespace Shared.Domain.Events;

public sealed class PriceSuggestedEvent : DomainEvent
{
    public Guid PropertyId { get; }
    public decimal SuggestedPrice { get; }
    public decimal ConfidenceScore { get; }
    public string ModelVersion { get; }

    public PriceSuggestedEvent(
        Guid propertyId,
        decimal suggestedPrice,
        decimal confidenceScore,
        string modelVersion)
    {
        PropertyId = propertyId;
        SuggestedPrice = suggestedPrice;
        ConfidenceScore = confidenceScore;
        ModelVersion = modelVersion;
    }
}

