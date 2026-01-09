using Shared.Domain.Events;

namespace SalesLoan.Domain.Events;

public sealed class BranchPerformanceUpdatedEvent : DomainEvent
{
    public Guid BranchId { get; }
    public int TotalSales { get; }
    public decimal TotalRevenue { get; }
    public int TotalLoans { get; }
    public DateTime UpdatedAt { get; }

    public BranchPerformanceUpdatedEvent(
        Guid branchId,
        int totalSales,
        decimal totalRevenue,
        int totalLoans,
        DateTime updatedAt)
    {
        BranchId = branchId;
        TotalSales = totalSales;
        TotalRevenue = totalRevenue;
        TotalLoans = totalLoans;
        UpdatedAt = updatedAt;
    }
}

