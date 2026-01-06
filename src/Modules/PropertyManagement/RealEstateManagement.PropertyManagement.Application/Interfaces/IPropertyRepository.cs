using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;

namespace RealEstate.Property.Application.Interfaces;

using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;

 

using RealEstateManagement.Property.Domain.ValueObjects;

public interface IPropertyRepository
{
    Task AddAsync(PropertyAggregate property, CancellationToken ct);
    Task<PropertyAggregate?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<PropertyAggregate>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<PropertyAggregate>> GetByStatusAsync(PropertyStatus? status, CancellationToken ct);
    Task<IEnumerable<PropertyAggregate>> GetByPriceRangeAsync(decimal? minPrice, decimal? maxPrice, CancellationToken ct);
    Task UpdateAsync(PropertyAggregate property, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}


 
 
