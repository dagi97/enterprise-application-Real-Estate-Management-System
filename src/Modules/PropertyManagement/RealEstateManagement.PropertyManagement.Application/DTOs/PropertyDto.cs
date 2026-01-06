namespace RealEstate.Property.Application.DTOs;

public sealed class PropertyDto
{
    public Guid Id { get; set; }
    public string City { get; set; } = null!;
    public string SubCity { get; set; } = null!;
    public string Street { get; set; } = null!;
    public decimal PriceAmount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
}
