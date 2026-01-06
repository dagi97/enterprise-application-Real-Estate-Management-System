namespace RealEstateManagement.Property.Domain.ValueObjects;

/// <summary>
/// Cross-context reference to Owner/Customer from Customer Management bounded context
/// </summary>
public sealed class OwnerId
{
    public Guid Value { get; }

    public OwnerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(value));
        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is OwnerId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}


