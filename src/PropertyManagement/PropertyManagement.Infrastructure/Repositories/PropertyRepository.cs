using Microsoft.EntityFrameworkCore;
using PropertyManagement.Domain.Aggregates;
using PropertyManagement.Domain.Repositories;
using PropertyManagement.Infrastructure.Data;
using Shared.Domain.Events;
using Shared.Infrastructure.Outbox;
using System.Text.Json;

namespace PropertyManagement.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly PropertyManagementDbContext _context;
    private readonly OutboxDbContext _outboxContext;

    public PropertyRepository(PropertyManagementDbContext context, OutboxDbContext outboxContext)
    {
        _context = context;
        _outboxContext = outboxContext;
    }

    public async Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Where(p => p.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _context.Properties.AddAsync(property, cancellationToken);
        await SaveDomainEventsAsync(property, cancellationToken);
    }

    public async Task UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        _context.Properties.Update(property);
        await SaveDomainEventsAsync(property, cancellationToken);
    }

    private async Task SaveDomainEventsAsync(Property property, CancellationToken cancellationToken)
    {
        var domainEvents = property.DomainEvents.ToList();
        property.ClearDomainEvents();

        // Use a transaction to ensure atomicity: both aggregate and outbox must be saved together
        // Since both contexts use the same database, we can share the transaction
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save aggregate changes first
            await _context.SaveChangesAsync(cancellationToken);

            // Save outbox messages in the same transaction
            foreach (var domainEvent in domainEvents)
            {
                var outboxMessage = new OutboxMessage
                {
                    Id = domainEvent.Id,
                    Type = domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().FullName ?? string.Empty,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = domainEvent.OccurredOn
                };

                await _outboxContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            }

            // Use the same transaction for outbox context (both use same database)
            _outboxContext.Database.UseTransaction(transaction.GetDbTransaction());
            await _outboxContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

