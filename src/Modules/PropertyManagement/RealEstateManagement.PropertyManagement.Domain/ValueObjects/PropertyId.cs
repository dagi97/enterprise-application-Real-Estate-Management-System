namespace RealEstateManagement.Property.Domain.ValueObjects;

public sealed class PropertyId
{
    public Guid Id { get; }

    public PropertyId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("PropertyId cannot be empty.", nameof(id));
        Id = id;
    }

    public override bool Equals(object? obj) =>
        obj is PropertyId other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
