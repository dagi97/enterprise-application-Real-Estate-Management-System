using Microsoft.EntityFrameworkCore;
using PricingValuation.Domain.Aggregates;
using PricingValuation.Domain.ValueObjects;

namespace PricingValuation.Infrastructure.Data;

public class PricingValuationDbContext : DbContext
{
    public PricingValuationDbContext(DbContextOptions<PricingValuationDbContext> options) : base(options)
    {
    }

    public DbSet<PriceEstimate> PriceEstimates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PricingValuation");
        
        modelBuilder.Entity<PriceEstimate>(entity =>
        {
            entity.ToTable("PriceEstimates");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.EstimatedPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.ConfidenceScore).HasColumnType("decimal(5,4)").IsRequired();
            entity.Property(e => e.ModelVersion).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EstimatedAt).IsRequired();
            entity.Property(e => e.IsApplied).IsRequired();
            
            entity.OwnsOne(e => e.Factors, factors =>
            {
                factors.Property(f => f.SizeFactor).HasColumnName("SizeFactor").HasColumnType("decimal(18,2)");
                factors.Property(f => f.LocationFactor).HasColumnName("LocationFactor").HasColumnType("decimal(18,2)");
                factors.Property(f => f.ConditionFactor).HasColumnName("ConditionFactor").HasColumnType("decimal(18,2)");
                factors.Property(f => f.BedroomFactor).HasColumnName("BedroomFactor").HasColumnType("decimal(18,2)");
                factors.Property(f => f.YearBuiltFactor).HasColumnName("YearBuiltFactor").HasColumnType("decimal(18,2)");
            });
            
            entity.Ignore(e => e.DomainEvents);
        });

        base.OnModelCreating(modelBuilder);
    }
}

