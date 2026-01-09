namespace Shared.Domain.Events;

public sealed class PropertySoldEvent : DomainEvent
{
    public Guid SaleId { get; }
    public Guid PropertyId { get; }
    public Guid BranchId { get; }
    public Guid CustomerId { get; }
    public decimal SaleAmount { get; }
    public DateTime SoldAt { get; }

    public PropertySoldEvent(
        Guid saleId,
        Guid propertyId,
        Guid branchId,
        Guid customerId,
        decimal saleAmount,
        DateTime soldAt)
    {
        SaleId = saleId;
        PropertyId = propertyId;
        BranchId = branchId;
        CustomerId = customerId;
        SaleAmount = saleAmount;
        SoldAt = soldAt;
    }
}

