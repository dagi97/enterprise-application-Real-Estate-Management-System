using Microsoft.EntityFrameworkCore;
using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Repositories;
using SalesLoan.Infrastructure.Data;
using Shared.Domain.Events;
using Shared.Infrastructure.Outbox;
using System.Text.Json;

namespace SalesLoan.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly SalesLoanDbContext _context;
    private readonly OutboxDbContext _outboxContext;

    public SaleRepository(SalesLoanDbContext context, OutboxDbContext outboxContext)
    {
        _context = context;
        _outboxContext = outboxContext;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.PropertyId == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await SaveDomainEventsAsync(sale, cancellationToken);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await SaveDomainEventsAsync(sale, cancellationToken);
    }

    private async Task SaveDomainEventsAsync(Sale sale, CancellationToken cancellationToken)
    {
        var domainEvents = sale.DomainEvents.ToList();
        sale.ClearDomainEvents();

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

