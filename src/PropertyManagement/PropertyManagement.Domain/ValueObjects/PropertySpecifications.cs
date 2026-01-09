using Shared.Domain.ValueObjects;

namespace PropertyManagement.Domain.ValueObjects;

public class PropertySpecifications : ValueObject
{
    public decimal Size { get; } // in square meters
    public int Bedrooms { get; }
    public int Bathrooms { get; }
    public string Condition { get; } // Excellent, Good, Fair, Poor
    public int? YearBuilt { get; }

    private PropertySpecifications() { }

    public PropertySpecifications(decimal size, int bedrooms, int bathrooms, string condition, int? yearBuilt = null)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be greater than zero", nameof(size));
        if (bedrooms < 0)
            throw new ArgumentException("Bedrooms cannot be negative", nameof(bedrooms));
        if (bathrooms < 0)
            throw new ArgumentException("Bathrooms cannot be negative", nameof(bathrooms));
        if (string.IsNullOrWhiteSpace(condition))
            throw new ArgumentException("Condition cannot be empty", nameof(condition));

        Size = size;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        Condition = condition;
        YearBuilt = yearBuilt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Size;
        yield return Bedrooms;
        yield return Bathrooms;
        yield return Condition;
        if (YearBuilt.HasValue)
            yield return YearBuilt.Value;
    }
}

