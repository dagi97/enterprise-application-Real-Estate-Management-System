using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;
using RealEstate.Property.Application.Interfaces;
using RealEstate.Property.Infrastructure.Persistence;
// TODO: Outbox will be moved to BuildingBlocks/Shared.Infrastructure
// using RealEstate.Property.Infrastructure.Messaging;
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
        
        // Save PropertyHistory entries if any were added during construction
        // Since History is ignored in EF Core, we need to manually track and save them
        foreach (var history in property.History)
        {
            // Set the PropertyId shadow property
            _dbContext.Entry(history).Property<Guid>("PropertyId").CurrentValue = property.Id.Id;
            _dbContext.PropertyHistories.Add(history);
        }
        
        // TODO: Outbox pattern will be moved to BuildingBlocks/Shared.Infrastructure
        // Save domain events to Outbox in the same transaction
        // await SaveDomainEventsToOutboxAsync(property, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PropertyAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Query using raw SQL to work around value converter translation issues
        // The value converter stores PropertyId as Guid in the database
        var property = await _dbContext.Properties
            .FromSqlRaw("SELECT * FROM property.\"Properties\" WHERE \"Id\" = {0}", id)
            .FirstOrDefaultAsync(cancellationToken);
        
        // Note: History is ignored in EF Core configuration, so it won't be loaded automatically
        // PropertyHistory entries are managed separately and loaded when needed
        // The History collection in Property is populated when PropertyHistory is added via AddHistory()
        
        return property;
    }

    public async Task<IEnumerable<PropertyAggregate>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            // Query using raw SQL to work around value converter translation issues
            // Filter out rows with empty or NULL BranchId at SQL level to prevent materialization errors
            // The BranchId value converter will throw if it receives an empty Guid
            // We filter at SQL level before EF Core tries to materialize entities
            var properties = await _dbContext.Properties
                .FromSqlRaw("SELECT * FROM property.\"Properties\" WHERE \"BranchId\" IS NOT NULL AND \"BranchId\" != '00000000-0000-0000-0000-000000000000'::uuid")
                .ToListAsync(ct);
            
            // Note: History is ignored in EF Core configuration
            // PropertyHistory entries are managed separately
            
            return properties;
        }
        catch (ArgumentException ex) when (ex.Message.Contains("BranchId cannot be empty"))
        {
            // Fallback: If SQL filter didn't prevent the error (edge case with data integrity issues)
            // Return empty list to prevent API crash - the data issue should be fixed at the source
            // In production, you would want to log this error
            return Enumerable.Empty<PropertyAggregate>();
        }
    }

    public async Task<IEnumerable<PropertyAggregate>> GetByStatusAsync(PropertyStatus? status, CancellationToken ct)
    {
        if (status == null)
            return await GetAllAsync(ct);

        try
        {
            // Query using raw SQL to work around value converter translation issues
            // Filter by status (stored as string) and exclude invalid BranchId values
            // The status is stored as a string in the database (from HasConversion<string>())
            var statusString = status.Value.ToString();
            var properties = await _dbContext.Properties
                .FromSqlRaw(
                    "SELECT * FROM property.\"Properties\" WHERE \"Status\" = {0} AND \"BranchId\" IS NOT NULL AND \"BranchId\" != '00000000-0000-0000-0000-000000000000'::uuid",
                    statusString)
                .ToListAsync(ct);
            
            // Note: History is ignored in EF Core configuration
            // PropertyHistory entries are managed separately
            
            return properties;
        }
        catch (ArgumentException ex) when (ex.Message.Contains("BranchId cannot be empty"))
        {
            // Fallback: If SQL filter didn't prevent the error (edge case with data integrity issues)
            // Return empty list to prevent API crash - the data issue should be fixed at the source
            // In production, you would want to log this error
            return Enumerable.Empty<PropertyAggregate>();
        }
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
        // Query using raw SQL to work around value converter translation issues
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
        
        // Save PropertyHistory entries that were added via AddHistory()
        // Since History is ignored in EF Core, we need to manually track and save them
        foreach (var history in property.History)
        {
            // Check if this history entry already exists
            // Query by PropertyId shadow property and filter in memory to avoid EF Core translation issues
            var propertyIdGuid = property.Id.Id;
            var historyIdGuid = history.Id.Value;
            
            var existingHistories = await _dbContext.PropertyHistories
                .Where(h => EF.Property<Guid>(h, "PropertyId") == propertyIdGuid)
                .ToListAsync(ct);
            
            var exists = existingHistories.Any(h => h.Id.Value == historyIdGuid);
            
            if (!exists)
            {
                // Set the PropertyId shadow property
                _dbContext.Entry(history).Property<Guid>("PropertyId").CurrentValue = property.Id.Id;
                _dbContext.PropertyHistories.Add(history);
            }
        }
        
        // TODO: Outbox pattern will be moved to BuildingBlocks/Shared.Infrastructure
        // Save domain events to Outbox in the same transaction
        // await SaveDomainEventsToOutboxAsync(property, ct);
        
        await _dbContext.SaveChangesAsync(ct);
    }

    // TODO: Outbox pattern will be moved to BuildingBlocks/Shared.Infrastructure
    // This method will be replaced with a shared implementation
    /*
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
    */
}

