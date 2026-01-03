namespace RealEstateManagement.Property.Domain.Entities;

public sealed class Owner
{
    public Guid OwnerId { get; private set; }   
    public string Name { get; private set; }

    public Owner(Guid ownerId, string name)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty.", nameof(ownerId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        OwnerId = ownerId;
        Name = name;
    }
}
