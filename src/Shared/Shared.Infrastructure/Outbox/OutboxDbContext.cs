using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Outbox;

namespace Shared.Infrastructure.Outbox;

public class OutboxDbContext : DbContext
{
    public OutboxDbContext(DbContextOptions<OutboxDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.HasIndex(e => new { e.ProcessedOn, e.OccurredOn });
        });

        base.OnModelCreating(modelBuilder);
    }
}

