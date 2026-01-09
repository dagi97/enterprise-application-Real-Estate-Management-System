using SalesLoan.Domain.Aggregates;

namespace SalesLoan.Domain.Repositories;

public interface ILoanApplicationRepository
{
    Task<LoanApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanApplication>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanApplication>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task AddAsync(LoanApplication application, CancellationToken cancellationToken = default);
    Task UpdateAsync(LoanApplication application, CancellationToken cancellationToken = default);
}

