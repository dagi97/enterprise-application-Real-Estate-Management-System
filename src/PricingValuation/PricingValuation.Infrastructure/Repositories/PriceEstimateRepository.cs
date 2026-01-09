using Microsoft.EntityFrameworkCore;
using PricingValuation.Domain.Aggregates;
using PricingValuation.Domain.Repositories;
using PricingValuation.Infrastructure.Data;
using Shared.Domain.Events;
using Shared.Infrastructure.Outbox;
using System.Text.Json;

namespace PricingValuation.Infrastructure.Repositories;

public class PriceEstimateRepository : IPriceEstimateRepository
{
    private readonly PricingValuationDbContext _context;
    private readonly OutboxDbContext _outboxContext;

    public PriceEstimateRepository(PricingValuationDbContext context, OutboxDbContext outboxContext)
    {
        _context = context;
        _outboxContext = outboxContext;
    }

    public async Task<PriceEstimate?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PriceEstimates
            .FirstOrDefaultAsync(e => e.PropertyId == propertyId, cancellationToken);
    }

    public async Task<PriceEstimate?> GetLatestByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PriceEstimates
            .Where(e => e.PropertyId == propertyId)
            .OrderByDescending(e => e.EstimatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(PriceEstimate estimate, CancellationToken cancellationToken = default)
    {
        await _context.PriceEstimates.AddAsync(estimate, cancellationToken);
        await SaveDomainEventsAsync(estimate, cancellationToken);
    }

    public async Task UpdateAsync(PriceEstimate estimate, CancellationToken cancellationToken = default)
    {
        _context.PriceEstimates.Update(estimate);
        await SaveDomainEventsAsync(estimate, cancellationToken);
    }

    private async Task SaveDomainEventsAsync(PriceEstimate estimate, CancellationToken cancellationToken)
    {
        var domainEvents = estimate.DomainEvents.ToList();
        estimate.ClearDomainEvents();

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

