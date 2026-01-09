using Microsoft.EntityFrameworkCore;
using PropertyManagement.Domain.Aggregates;
using PropertyManagement.Domain.Enums;
using PropertyManagement.Domain.ValueObjects;

namespace PropertyManagement.Infrastructure.Data;

public class PropertyManagementDbContext : DbContext
{
    public PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PropertyManagement");
        
        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Properties");
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.BranchId).IsRequired();
            entity.Property(p => p.PropertyType).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Status).HasConversion<string>();
            entity.Property(p => p.ListedPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SuggestedPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Description).HasMaxLength(2000);
            entity.Property(p => p.RegisteredAt).IsRequired();
            
            // Owned Types - Value Objects
            entity.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").IsRequired().HasMaxLength(200);
                address.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("State").IsRequired().HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("ZipCode").IsRequired().HasMaxLength(20);
                address.Property(a => a.Country).HasColumnName("Country").IsRequired().HasMaxLength(100);
            });
            
            entity.OwnsOne(p => p.Specifications, specs =>
            {
                specs.Property(s => s.Size).HasColumnName("Size").IsRequired().HasColumnType("decimal(18,2)");
                specs.Property(s => s.Bedrooms).HasColumnName("Bedrooms").IsRequired();
                specs.Property(s => s.Bathrooms).HasColumnName("Bathrooms").IsRequired();
                specs.Property(s => s.Condition).HasColumnName("Condition").IsRequired().HasMaxLength(50);
                specs.Property(s => s.YearBuilt).HasColumnName("YearBuilt");
            });
            
            entity.Ignore(p => p.DomainEvents);
        });

        base.OnModelCreating(modelBuilder);
    }
}

