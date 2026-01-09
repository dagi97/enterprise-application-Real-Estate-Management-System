using SalesLoan.Domain.Enums;
using SalesLoan.Domain.Rules;
using SalesLoan.Domain.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Events;

namespace SalesLoan.Domain.Aggregates;

public class LoanApplication : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public Guid BranchId { get; private set; }
    public CustomerInfo Customer { get; private set; } = null!;
    public Money RequestedAmount { get; private set; } = null!;
    public Money? ApprovedAmount { get; private set; }
    public LoanStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime AppliedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public bool IsPropertyReserved { get; private set; }

    private LoanApplication() { }

    private LoanApplication(
        Guid id,
        Guid propertyId,
        Guid branchId,
        CustomerInfo customer,
        Money requestedAmount)
    {
        Id = id;
        PropertyId = propertyId;
        BranchId = branchId;
        Customer = customer;
        RequestedAmount = requestedAmount;
        Status = LoanStatus.Pending;
        AppliedAt = DateTime.UtcNow;
        IsPropertyReserved = false;
    }

    public static LoanApplication Create(
        Guid propertyId,
        Guid branchId,
        CustomerInfo customer,
        Money requestedAmount)
    {
        var application = new LoanApplication(
            Guid.NewGuid(),
            propertyId,
            branchId,
            customer,
            requestedAmount);

        application.RaiseDomainEvent(new LoanRequestedEvent(
            application.Id,
            application.PropertyId,
            application.BranchId,
            application.Customer.Email,
            application.RequestedAmount.Amount));

        return application;
    }

    public void Approve(Money approvedAmount)
    {
        if (Status != LoanStatus.Pending && Status != LoanStatus.UnderReview)
            throw new InvalidOperationException($"Cannot approve loan with status {Status}");

        CheckRule(new ApprovedAmountMustNotExceedRequestedAmountRule(RequestedAmount.Amount, approvedAmount.Amount));

        ApprovedAmount = approvedAmount;
        Status = LoanStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        ApprovedAt = DateTime.UtcNow;

        RaiseDomainEvent(new LoanApprovedEvent(
            Id,
            PropertyId,
            BranchId,
            ApprovedAmount.Amount));
    }

    public void Reject(string reason)
    {
        if (Status != LoanStatus.Pending && Status != LoanStatus.UnderReview)
            throw new InvalidOperationException($"Cannot reject loan with status {Status}");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason cannot be empty", nameof(reason));

        Status = LoanStatus.Rejected;
        RejectionReason = reason;
        ReviewedAt = DateTime.UtcNow;
    }

    public void MarkAsUnderReview()
    {
        if (Status != LoanStatus.Pending)
            throw new InvalidOperationException($"Cannot mark as under review with status {Status}");

        Status = LoanStatus.UnderReview;
    }

    public void ReserveProperty()
    {
        if (IsPropertyReserved)
            return;

        if (Status != LoanStatus.Approved)
            throw new InvalidOperationException("Property can only be reserved for approved loans");

        IsPropertyReserved = true;
        RaiseDomainEvent(new PropertyReservedEvent(PropertyId, BranchId, Id));
    }

    public void MarkAsDisbursed()
    {
        if (Status != LoanStatus.Approved)
            throw new InvalidOperationException($"Cannot disburse loan with status {Status}");

        Status = LoanStatus.Disbursed;
    }

    public void Close()
    {
        if (Status != LoanStatus.Disbursed)
            throw new InvalidOperationException($"Cannot close loan with status {Status}");

        Status = LoanStatus.Closed;
    }
}

