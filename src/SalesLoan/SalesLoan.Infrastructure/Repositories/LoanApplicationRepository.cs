using Microsoft.EntityFrameworkCore;
using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Repositories;
using SalesLoan.Infrastructure.Data;
using Shared.Domain.Events;
using Shared.Infrastructure.Outbox;
using System.Text.Json;

namespace SalesLoan.Infrastructure.Repositories;

public class LoanApplicationRepository : ILoanApplicationRepository
{
    private readonly SalesLoanDbContext _context;
    private readonly OutboxDbContext _outboxContext;

    public LoanApplicationRepository(SalesLoanDbContext context, OutboxDbContext outboxContext)
    {
        _context = context;
        _outboxContext = outboxContext;
    }

    public async Task<LoanApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LoanApplications
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<LoanApplication>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.LoanApplications
            .Where(l => l.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LoanApplication>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.LoanApplications
            .Where(l => l.PropertyId == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LoanApplication application, CancellationToken cancellationToken = default)
    {
        await _context.LoanApplications.AddAsync(application, cancellationToken);
        await SaveDomainEventsAsync(application, cancellationToken);
    }

    public async Task UpdateAsync(LoanApplication application, CancellationToken cancellationToken = default)
    {
        _context.LoanApplications.Update(application);
        await SaveDomainEventsAsync(application, cancellationToken);
    }

    private async Task SaveDomainEventsAsync(LoanApplication application, CancellationToken cancellationToken)
    {
        var domainEvents = application.DomainEvents.ToList();
        application.ClearDomainEvents();

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

