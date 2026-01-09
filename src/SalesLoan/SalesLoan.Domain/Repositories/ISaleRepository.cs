using SalesLoan.Domain.Aggregates;

namespace SalesLoan.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
}

