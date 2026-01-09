using Microsoft.EntityFrameworkCore;
using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Enums;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Infrastructure.Data;

public class SalesLoanDbContext : DbContext
{
    public SalesLoanDbContext(DbContextOptions<SalesLoanDbContext> options) : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications { get; set; }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SalesLoan");
        
        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.ToTable("LoanApplications");
            entity.HasKey(l => l.Id);
            
            entity.Property(l => l.PropertyId).IsRequired();
            entity.Property(l => l.BranchId).IsRequired();
            entity.Property(l => l.Status).HasConversion<string>();
            entity.Property(l => l.RejectionReason).HasMaxLength(1000);
            entity.Property(l => l.AppliedAt).IsRequired();
            
            entity.OwnsOne(l => l.Customer, customer =>
            {
                customer.Property(c => c.FirstName).HasColumnName("CustomerFirstName").IsRequired().HasMaxLength(100);
                customer.Property(c => c.LastName).HasColumnName("CustomerLastName").IsRequired().HasMaxLength(100);
                customer.Property(c => c.Email).HasColumnName("CustomerEmail").IsRequired().HasMaxLength(200);
                customer.Property(c => c.PhoneNumber).HasColumnName("CustomerPhoneNumber").IsRequired().HasMaxLength(50);
                customer.Property(c => c.Address).HasColumnName("CustomerAddress").HasMaxLength(500);
            });
            
            entity.OwnsOne(l => l.RequestedAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("RequestedAmount").IsRequired().HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("RequestedCurrency").IsRequired().HasMaxLength(10);
            });
            
            entity.OwnsOne(l => l.ApprovedAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("ApprovedAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("ApprovedCurrency").HasMaxLength(10);
            });
            
            entity.Ignore(l => l.DomainEvents);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(s => s.Id);
            
            entity.Property(s => s.PropertyId).IsRequired();
            entity.Property(s => s.BranchId).IsRequired();
            entity.Property(s => s.CustomerId).IsRequired();
            entity.Property(s => s.Status).HasConversion<string>();
            entity.Property(s => s.SaleDate).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.Notes).HasMaxLength(2000);
            
            entity.OwnsOne(s => s.Customer, customer =>
            {
                customer.Property(c => c.FirstName).HasColumnName("CustomerFirstName").IsRequired().HasMaxLength(100);
                customer.Property(c => c.LastName).HasColumnName("CustomerLastName").IsRequired().HasMaxLength(100);
                customer.Property(c => c.Email).HasColumnName("CustomerEmail").IsRequired().HasMaxLength(200);
                customer.Property(c => c.PhoneNumber).HasColumnName("CustomerPhoneNumber").IsRequired().HasMaxLength(50);
                customer.Property(c => c.Address).HasColumnName("CustomerAddress").HasMaxLength(500);
            });
            
            entity.OwnsOne(s => s.SaleAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("SaleAmount").IsRequired().HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("SaleCurrency").IsRequired().HasMaxLength(10);
            });
            
            entity.OwnsOne(s => s.Commission, money =>
            {
                money.Property(m => m.Amount).HasColumnName("CommissionAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("CommissionCurrency").HasMaxLength(10);
            });
            
            entity.Ignore(s => s.DomainEvents);
        });

        base.OnModelCreating(modelBuilder);
    }
}

