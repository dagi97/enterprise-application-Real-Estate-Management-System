using Shared.Domain.Events;

namespace SalesLoan.Domain.Events;

public sealed class LoanApprovedEvent : DomainEvent
{
    public Guid LoanApplicationId { get; }
    public Guid PropertyId { get; }
    public Guid BranchId { get; }
    public decimal ApprovedAmount { get; }

    public LoanApprovedEvent(
        Guid loanApplicationId,
        Guid propertyId,
        Guid branchId,
        decimal approvedAmount)
    {
        LoanApplicationId = loanApplicationId;
        PropertyId = propertyId;
        BranchId = branchId;
        ApprovedAmount = approvedAmount;
    }
}

