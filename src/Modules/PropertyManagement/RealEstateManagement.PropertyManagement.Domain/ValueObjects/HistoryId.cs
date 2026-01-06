namespace RealEstateManagement.Property.Domain.ValueObjects;

public sealed class HistoryId
{
    public Guid Value { get; }

    public HistoryId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("HistoryId cannot be empty.", nameof(value));
        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is HistoryId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}


