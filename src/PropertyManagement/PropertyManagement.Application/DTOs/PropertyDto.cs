namespace PropertyManagement.Application.DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Size { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int? YearBuilt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? ListedPrice { get; set; }
    public decimal? SuggestedPrice { get; set; }
    public string? Description { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

