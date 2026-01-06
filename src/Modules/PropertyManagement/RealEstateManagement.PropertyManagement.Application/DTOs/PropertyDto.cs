namespace RealEstate.Property.Application.DTOs;

public sealed class PropertyDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string City { get; set; } = null!;
    public string SubCity { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public decimal PriceAmount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Guid? OwnerId { get; set; }
    public decimal SizeSqMeters { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int YearBuilt { get; set; }
}
