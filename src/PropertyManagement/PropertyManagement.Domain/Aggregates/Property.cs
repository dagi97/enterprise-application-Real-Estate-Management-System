using PropertyManagement.Domain.Enums;
using PropertyManagement.Domain.Rules;
using PropertyManagement.Domain.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Events;

namespace PropertyManagement.Domain.Aggregates;

public class Property : AggregateRoot<Guid>
{
    public Guid BranchId { get; private set; }
    public string PropertyType { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;
    public PropertySpecifications Specifications { get; private set; } = null!;
    public PropertyStatus Status { get; private set; }
    public decimal? ListedPrice { get; private set; }
    public decimal? SuggestedPrice { get; private set; }
    public string? Description { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Property() { }

    private Property(
        Guid id,
        Guid branchId,
        string propertyType,
        Address address,
        PropertySpecifications specifications,
        string? description = null)
    {
        Id = id;
        BranchId = branchId;
        PropertyType = propertyType;
        Address = address;
        Specifications = specifications;
        Description = description;
        Status = PropertyStatus.Draft;
        RegisteredAt = DateTime.UtcNow;
    }

    public static Property Create(
        Guid branchId,
        string propertyType,
        Address address,
        PropertySpecifications specifications,
        string? description = null)
    {
        var property = new Property(
            Guid.NewGuid(),
            branchId,
            propertyType,
            address,
            specifications,
            description);

        property.Status = PropertyStatus.Registered;
        property.RaiseDomainEvent(new PropertyRegisteredEvent(
            property.Id,
            property.BranchId,
            property.PropertyType,
            property.Specifications.Size,
            property.Specifications.Bedrooms,
            property.Address.City,
            property.Address.State));

        return property;
    }

    public void UpdatePrice(decimal price)
    {
        CheckRule(new PropertyMustHaveValidPriceRule(price));
        ListedPrice = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSuggestedPrice(decimal suggestedPrice)
    {
        if (suggestedPrice <= 0)
            throw new ArgumentException("Suggested price must be greater than zero", nameof(suggestedPrice));

        SuggestedPrice = suggestedPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(PropertyStatus newStatus)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PropertyStatusChangedEvent(
            Id,
            oldStatus.ToString(),
            newStatus.ToString()));
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reserve()
    {
        if (Status != PropertyStatus.Available)
            throw new InvalidOperationException($"Cannot reserve property with status {Status}");

        ChangeStatus(PropertyStatus.Reserved);
    }

    public void MarkAsSold()
    {
        if (Status != PropertyStatus.Reserved && Status != PropertyStatus.Available)
            throw new InvalidOperationException($"Cannot mark property as sold with status {Status}");

        ChangeStatus(PropertyStatus.Sold);
    }

    public void MakeAvailable()
    {
        if (Status != PropertyStatus.PendingApproval && Status != PropertyStatus.Reserved)
            throw new InvalidOperationException($"Cannot make property available with status {Status}");

        ChangeStatus(PropertyStatus.Available);
    }
}

