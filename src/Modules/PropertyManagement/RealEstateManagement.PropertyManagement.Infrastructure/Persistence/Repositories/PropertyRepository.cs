using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;
using RealEstate.Property.Application.Interfaces;
using RealEstate.Property.Infrastructure.Persistence;
using RealEstate.Property.Infrastructure.Messaging;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Shared.Domain.ValueObjects;

namespace RealEstate.Property.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly PropertyDbContext _dbContext;

    public PropertyRepository(PropertyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PropertyAggregate property, CancellationToken cancellationToken)
    {
        // Check if property with this ID already exists
        var existing = await GetByIdAsync(property.Id.Id, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Property with ID {property.Id.Id} already exists.");
        }
        
        _dbContext.Properties.Add(property);
        
        // Save domain events to Outbox in the same transaction
        await SaveDomainEventsToOutboxAsync(property, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PropertyAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        
        return await _dbContext.Properties
            .FromSqlRaw("SELECT * FROM property.\"Properties\" WHERE \"Id\" = {0}", id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<PropertyAggregate>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Properties.ToListAsync(ct);
    }

    public async Task<IEnumerable<PropertyAggregate>> GetByStatusAsync(PropertyStatus? status, CancellationToken ct)
    {
        if (status == null)
            return await GetAllAsync(ct);

        return await _dbContext.Properties
            .Where(p => p.Status == status.Value)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<PropertyAggregate>> GetByPriceRangeAsync(decimal? minPrice, decimal? maxPrice, CancellationToken ct)
    {
        var query = _dbContext.Properties.AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);

        return await query.ToListAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
         
        var property = await _dbContext.Properties
            .FromSqlRaw("SELECT * FROM property.\"Properties\" WHERE \"Id\" = {0}", id)
            .FirstOrDefaultAsync(ct);
        if (property != null)
        {
            _dbContext.Properties.Remove(property);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateAsync(PropertyAggregate property, CancellationToken ct)
    {
        _dbContext.Properties.Update(property);
        
         await SaveDomainEventsToOutboxAsync(property, ct);
        
        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task SaveDomainEventsToOutboxAsync(PropertyAggregate property, CancellationToken cancellationToken)
    {
        var domainEvents = property.DomainEvents;
        if (domainEvents == null || domainEvents.Count == 0)
            return;

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredOn = DateTime.UtcNow
            };

            _dbContext.OutboxMessages.Add(outboxMessage);
        }
    }
}

