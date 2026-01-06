namespace RealEstate.Shared.Domain.ValueObjects;

/// <summary>
/// Shared Kernel Value Object - Property physical characteristics
/// Shared with Pricing module for price estimation
/// </summary>
public sealed class PropertyFeatures
{
    public decimal SizeSqMeters { get; }
    public int Bedrooms { get; }
    public int Bathrooms { get; }
    public int YearBuilt { get; }

    public PropertyFeatures(decimal sizeSqMeters, int bedrooms, int bathrooms, int yearBuilt)
    {
        if (sizeSqMeters <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(sizeSqMeters));
        if (bedrooms < 0)
            throw new ArgumentException("Bedrooms cannot be negative.", nameof(bedrooms));
        if (bathrooms < 0)
            throw new ArgumentException("Bathrooms cannot be negative.", nameof(bathrooms));
        if (yearBuilt < 1800 || yearBuilt > DateTime.Now.Year)
            throw new ArgumentException("Year built must be a valid year.", nameof(yearBuilt));

        SizeSqMeters = sizeSqMeters;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        YearBuilt = yearBuilt;
    }

    public override bool Equals(object? obj) =>
        obj is PropertyFeatures other &&
        SizeSqMeters == other.SizeSqMeters &&
        Bedrooms == other.Bedrooms &&
        Bathrooms == other.Bathrooms &&
        YearBuilt == other.YearBuilt;

    public override int GetHashCode() =>
        HashCode.Combine(SizeSqMeters, Bedrooms, Bathrooms, YearBuilt);
}

