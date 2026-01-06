using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstateManagement.Property.Domain.Entities;
// TODO: Outbox will be moved to BuildingBlocks/Shared.Infrastructure
// using RealEstate.Property.Infrastructure.Messaging;
using RealEstate.Shared.Domain.ValueObjects;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;

namespace RealEstate.Property.Infrastructure.Persistence;

public sealed class PropertyDbContext : DbContext
{
    public const string SchemaName = "property";
    
    public DbSet<PropertyAggregate> Properties { get; set; } = null!;
    public DbSet<PropertyHistory> PropertyHistories { get; set; } = null!;
    // TODO: Outbox will be moved to BuildingBlocks/Shared.Infrastructure
    // public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema for all entities in this context
        modelBuilder.HasDefaultSchema(SchemaName);

        // Configure Property aggregate
        modelBuilder.Entity<PropertyAggregate>(entity =>
        {
            entity.ToTable("Properties", SchemaName);
            
            // Configure PropertyId value object as primary key
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .HasConversion(
                    id => id.Id,
                    guid => new PropertyId(guid))
                .HasColumnName("Id")
                .ValueGeneratedNever();
            
            // Configure BranchId value object
            entity.Property(p => p.BranchId)
                .HasConversion(
                    id => id.Value,
                    guid => new BranchId(guid))
                .HasColumnName("BranchId")
                .IsRequired();
            
            // Configure OwnerId value object (nullable)
            entity.Property(p => p.OwnerId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?)null,
                    guid => guid.HasValue ? new OwnerId(guid.Value) : null)
                .HasColumnName("OwnerId");
            
            // Configure value objects as owned types
            entity.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(500);
                address.Property(a => a.City).HasColumnName("City").HasMaxLength(200);
                address.Property(a => a.SubCity).HasColumnName("SubCity").HasMaxLength(200);
                address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            });
            
            entity.OwnsOne(p => p.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnName("PriceAmount").HasPrecision(18, 2);
                price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(10);
            });
            
            entity.OwnsOne(p => p.Features, features =>
            {
                features.Property(f => f.SizeSqMeters).HasColumnName("SizeSqMeters").HasPrecision(18, 2);
                features.Property(f => f.Bedrooms).HasColumnName("Bedrooms");
                features.Property(f => f.Bathrooms).HasColumnName("Bathrooms");
                features.Property(f => f.YearBuilt).HasColumnName("YearBuilt");
            });
            
            // Configure Status enum
            entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            
            // Ignore History collection - we'll manage PropertyHistory separately
            // This avoids EF Core's limitation with value object foreign keys
            entity.Ignore(p => p.History);
            
            // Ignore domain events (they're not persisted)
            entity.Ignore(p => p.DomainEvents);
        });

        // Configure PropertyHistory entity
        modelBuilder.Entity<PropertyHistory>(entity =>
        {
            entity.ToTable("PropertyHistories", SchemaName);
            
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Id)
                .HasConversion(
                    id => id.Value,
                    guid => new HistoryId(guid))
                .HasColumnName("Id")
                .ValueGeneratedNever();
            
            entity.Property(h => h.ChangeType)
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(h => h.OldValue)
                .HasMaxLength(1000);
            
            entity.Property(h => h.NewValue)
                .HasMaxLength(1000);
            
            entity.Property(h => h.ChangedBy)
                .HasMaxLength(200)
                .IsRequired();
            
            entity.Property(h => h.ChangedAt)
                .IsRequired();
            
            // Foreign key to Property (shadow property for EF Core)
            // This Guid references Property.Id.Id (the underlying Guid value stored in the "Id" column)
            // Note: We can't use a navigation property relationship due to EF Core's limitation
            // with value object foreign keys. The relationship will be managed manually in queries.
            entity.Property<Guid>("PropertyId")
                .IsRequired()
                .HasColumnName("PropertyId");
            
            entity.HasIndex("PropertyId");
            
            // Foreign key constraint will be added in migration SQL
            // PropertyHistories.PropertyId -> Properties.Id (both are Guid in database)
        });

        // TODO: Outbox will be moved to BuildingBlocks/Shared.Infrastructure
        // Configure Outbox table
        /*
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages", SchemaName);
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).ValueGeneratedNever();
            entity.Property(o => o.Type).HasMaxLength(500).IsRequired();
            entity.Property(o => o.Payload).HasColumnType("jsonb").IsRequired();
            entity.Property(o => o.OccurredOn).IsRequired();
            entity.Property(o => o.ProcessedOn);
            entity.HasIndex(o => o.ProcessedOn);
        });
        */

        base.OnModelCreating(modelBuilder);
    }
}
