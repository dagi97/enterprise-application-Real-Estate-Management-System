using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Property.Domain.ValueObjects;
using RealEstate.Property.Infrastructure.Messaging;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;

namespace RealEstate.Property.Infrastructure.Persistence;

public sealed class PropertyDbContext : DbContext
{
    public const string SchemaName = "property";
    
    public DbSet<PropertyAggregate> Properties { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.HasDefaultSchema(SchemaName);

         modelBuilder.Entity<PropertyAggregate>(entity =>
        {
            entity.ToTable("Properties", SchemaName);
            
             entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .HasConversion(
                    id => id.Id,
                    guid => new PropertyId(guid))
                .HasColumnName("Id")
                .ValueGeneratedNever();
            
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
            
             entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            
             entity.OwnsOne(p => p.Owner, owner =>
            {
                owner.Property(o => o.OwnerId).HasColumnName("OwnerId");
                owner.Property(o => o.Name).HasColumnName("OwnerName").HasMaxLength(200);
            });
            
             entity.Ignore(p => p.DomainEvents);
        });

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

        base.OnModelCreating(modelBuilder);
    }
}

