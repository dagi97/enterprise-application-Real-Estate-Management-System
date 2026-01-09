namespace Shared.Domain.Events;

public sealed class PropertyReservedEvent : DomainEvent
{
    public Guid PropertyId { get; }
    public Guid BranchId { get; }
    public Guid LoanApplicationId { get; }

    public PropertyReservedEvent(Guid propertyId, Guid branchId, Guid loanApplicationId)
    {
        PropertyId = propertyId;
        BranchId = branchId;
        LoanApplicationId = loanApplicationId;
    }
}

