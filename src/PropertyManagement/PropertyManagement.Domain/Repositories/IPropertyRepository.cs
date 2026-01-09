using PropertyManagement.Domain.Aggregates;

namespace PropertyManagement.Domain.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
}

