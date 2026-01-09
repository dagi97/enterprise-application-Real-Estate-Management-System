using Shared.Domain.Events;

namespace SalesLoan.Domain.Events;

public sealed class LoanRequestedEvent : DomainEvent
{
    public Guid LoanApplicationId { get; }
    public Guid PropertyId { get; }
    public Guid BranchId { get; }
    public string CustomerEmail { get; }
    public decimal LoanAmount { get; }

    public LoanRequestedEvent(
        Guid loanApplicationId,
        Guid propertyId,
        Guid branchId,
        string customerEmail,
        decimal loanAmount)
    {
        LoanApplicationId = loanApplicationId;
        PropertyId = propertyId;
        BranchId = branchId;
        CustomerEmail = customerEmail;
        LoanAmount = loanAmount;
    }
}

