namespace RealEstateManagement.Property.Domain.ValueObjects;

public sealed class Address
{
    public string Street { get; }
    public string City { get; }
    public string SubCity { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string subCity, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));
        if (string.IsNullOrWhiteSpace(subCity))
            throw new ArgumentException("SubCity cannot be empty.", nameof(subCity));
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be empty.", nameof(zipCode));

        Street = street;
        City = city;
        SubCity = subCity;
        ZipCode = zipCode;
    }

    public override bool Equals(object? obj) =>
        obj is Address other &&
        Street == other.Street &&
        City == other.City &&
        SubCity == other.SubCity &&
        ZipCode == other.ZipCode;

    public override int GetHashCode() =>
        HashCode.Combine(Street, City, SubCity, ZipCode);
}
