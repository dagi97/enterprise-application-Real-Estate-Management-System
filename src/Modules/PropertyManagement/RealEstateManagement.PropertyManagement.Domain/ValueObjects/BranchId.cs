namespace RealEstateManagement.Property.Domain.ValueObjects;

/// <summary>
/// Cross-context reference to Branch from Branch Operations bounded context
/// </summary>
public sealed class BranchId
{
    public Guid Value { get; }

    public BranchId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("BranchId cannot be empty.", nameof(value));
        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is BranchId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}


