using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Enums;
using SalesLoan.Domain.Events;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Domain.Tests;

public class SaleTests
{
    [Fact]
    public void Create_ShouldCreateSaleWithPendingStatus()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var saleAmount = new Money(500000m, "USD");

        // Act
        var sale = Sale.Create(propertyId, branchId, customerId, customer, saleAmount, "Test notes");

        // Assert
        Assert.NotNull(sale);
        Assert.Equal(propertyId, sale.PropertyId);
        Assert.Equal(branchId, sale.BranchId);
        Assert.Equal(customerId, sale.CustomerId);
        Assert.Equal(customer, sale.Customer);
        Assert.Equal(saleAmount, sale.SaleAmount);
        Assert.Equal(SaleStatus.Pending, sale.Status);
        Assert.Equal("Test notes", sale.Notes);
        Assert.Null(sale.Commission);
    }

    [Fact]
    public void Create_ShouldNotRaiseEvent()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var saleAmount = new Money(500000m, "USD");

        // Act
        var sale = Sale.Create(propertyId, branchId, customerId, customer, saleAmount);

        // Assert
        Assert.Empty(sale.DomainEvents);
    }

    [Fact]
    public void Confirm_WhenPending_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var sale = CreateTestSale();

        // Act
        sale.Confirm();

        // Assert
        Assert.Equal(SaleStatus.Confirmed, sale.Status);
    }

    [Fact]
    public void Confirm_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.Confirm();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Confirm());
        Assert.Contains("Cannot confirm sale with status Confirmed", exception.Message);
    }

    [Fact]
    public void Complete_WhenConfirmed_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.Confirm();

        // Act
        sale.Complete();

        // Assert
        Assert.Equal(SaleStatus.Completed, sale.Status);
        Assert.Single(sale.DomainEvents);
        
        var @event = sale.DomainEvents.First() as PropertySoldEvent;
        Assert.NotNull(@event);
        Assert.Equal(sale.Id, @event.SaleId);
        Assert.Equal(sale.PropertyId, @event.PropertyId);
        Assert.Equal(sale.BranchId, @event.BranchId);
        Assert.Equal(sale.CustomerId, @event.CustomerId);
        Assert.Equal(500000m, @event.SaleAmount);
    }

    [Fact]
    public void Complete_WhenNotConfirmed_ShouldThrowException()
    {
        // Arrange
        var sale = CreateTestSale();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Complete());
        Assert.Contains("Cannot complete sale with status Pending", exception.Message);
    }

    [Fact]
    public void Cancel_WhenPending_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var sale = CreateTestSale();
        var reason = "Customer changed mind";

        // Act
        sale.Cancel(reason);

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.Equal(reason, sale.Notes);
    }

    [Fact]
    public void Cancel_WhenConfirmed_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.Confirm();
        var reason = "Customer changed mind";

        // Act
        sale.Cancel(reason);

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.Equal(reason, sale.Notes);
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var sale = CreateTestSale();
        sale.Confirm();
        sale.Complete();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Cancel("Reason"));
        Assert.Contains("Cannot cancel a completed sale", exception.Message);
    }

    [Fact]
    public void SetCommission_ShouldSetCommission()
    {
        // Arrange
        var sale = CreateTestSale();
        var commission = new Money(25000m, "USD");

        // Act
        sale.SetCommission(commission);

        // Assert
        Assert.Equal(commission, sale.Commission);
    }

    private Sale CreateTestSale()
    {
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var saleAmount = new Money(500000m, "USD");
        return Sale.Create(propertyId, branchId, customerId, customer, saleAmount);
    }
}

