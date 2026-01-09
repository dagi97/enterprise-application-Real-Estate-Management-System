using SalesLoan.Domain.Enums;
using SalesLoan.Domain.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Events;

namespace SalesLoan.Domain.Aggregates;

public class Sale : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public CustomerInfo Customer { get; private set; } = null!;
    public Money SaleAmount { get; private set; } = null!;
    public Money? Commission { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTime SaleDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Notes { get; private set; }

    private Sale() { }

    private Sale(
        Guid id,
        Guid propertyId,
        Guid branchId,
        Guid customerId,
        CustomerInfo customer,
        Money saleAmount,
        string? notes = null)
    {
        Id = id;
        PropertyId = propertyId;
        BranchId = branchId;
        CustomerId = customerId;
        Customer = customer;
        SaleAmount = saleAmount;
        Status = SaleStatus.Pending;
        SaleDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        Notes = notes;
    }

    public static Sale Create(
        Guid propertyId,
        Guid branchId,
        Guid customerId,
        CustomerInfo customer,
        Money saleAmount,
        string? notes = null)
    {
        var sale = new Sale(
            Guid.NewGuid(),
            propertyId,
            branchId,
            customerId,
            customer,
            saleAmount,
            notes);

        return sale;
    }

    public void Confirm()
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm sale with status {Status}");

        Status = SaleStatus.Confirmed;
    }

    public void Complete()
    {
        if (Status != SaleStatus.Confirmed)
            throw new InvalidOperationException($"Cannot complete sale with status {Status}");

        Status = SaleStatus.Completed;

        RaiseDomainEvent(new PropertySoldEvent(
            Id,
            PropertyId,
            BranchId,
            CustomerId,
            SaleAmount.Amount,
            SaleDate));
    }

    public void Cancel(string reason)
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed sale");

        Status = SaleStatus.Cancelled;
        Notes = reason;
    }

    public void SetCommission(Money commission)
    {
        Commission = commission;
    }
}

