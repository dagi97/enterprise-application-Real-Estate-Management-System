using Shared.Domain.ValueObjects;

namespace PricingValuation.Domain.ValueObjects;

public class PricingFactors : ValueObject
{
    public decimal SizeFactor { get; }
    public decimal LocationFactor { get; }
    public decimal ConditionFactor { get; }
    public decimal BedroomFactor { get; }
    public decimal YearBuiltFactor { get; }

    private PricingFactors() { }

    public PricingFactors(
        decimal sizeFactor,
        decimal locationFactor,
        decimal conditionFactor,
        decimal bedroomFactor,
        decimal yearBuiltFactor)
    {
        SizeFactor = sizeFactor;
        LocationFactor = locationFactor;
        ConditionFactor = conditionFactor;
        BedroomFactor = bedroomFactor;
        YearBuiltFactor = yearBuiltFactor;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SizeFactor;
        yield return LocationFactor;
        yield return ConditionFactor;
        yield return BedroomFactor;
        yield return YearBuiltFactor;
    }
}

