using SalesLoan.Domain.Events;
using SalesLoan.Domain.Repositories;
using Shared.Domain.Events;
using Shared.Domain.EventHandlers;

namespace SalesLoan.Application.EventHandlers;

public class PropertySoldEventHandler : IEventSubscriber<PropertySoldEvent>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IEventBus _eventBus;

    public PropertySoldEventHandler(
        ISaleRepository saleRepository,
        ILoanApplicationRepository loanApplicationRepository,
        IEventBus eventBus)
    {
        _saleRepository = saleRepository;
        _loanApplicationRepository = loanApplicationRepository;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(PropertySoldEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Calculate branch performance metrics
        var branchId = domainEvent.BranchId;
        
        // Get all completed sales for this branch
        var sales = await _saleRepository.GetByBranchIdAsync(branchId, cancellationToken);
        var completedSales = sales.Where(s => s.Status == SalesLoan.Domain.Enums.SaleStatus.Completed);
        var totalSales = completedSales.Count();
        var totalRevenue = completedSales.Sum(s => s.SaleAmount.Amount);
        
        // Get all approved loans for this branch
        var loans = await _loanApplicationRepository.GetByBranchIdAsync(branchId, cancellationToken);
        var approvedLoans = loans.Where(l => l.Status == SalesLoan.Domain.Enums.LoanStatus.Approved || 
                                               l.Status == SalesLoan.Domain.Enums.LoanStatus.Disbursed ||
                                               l.Status == SalesLoan.Domain.Enums.LoanStatus.Closed);
        var totalLoans = approvedLoans.Count();
        
        // Publish branch performance updated event
        var branchPerformanceEvent = new BranchPerformanceUpdatedEvent(
            branchId,
            totalSales,
            totalRevenue,
            totalLoans,
            DateTime.UtcNow);
        
        await _eventBus.PublishAsync(branchPerformanceEvent, cancellationToken);
    }
}

