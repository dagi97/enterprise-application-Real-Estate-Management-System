using PropertyManagement.Domain.Aggregates;
using PropertyManagement.Domain.Enums;
using PropertyManagement.Domain.Events;
using PropertyManagement.Domain.ValueObjects;
using Shared.Domain.Rules;

namespace PropertyManagement.Domain.Tests;

public class PropertyTests
{
    [Fact]
    public void Create_ShouldCreatePropertyWithRegisteredStatus()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var specifications = new PropertySpecifications(2000m, 3, 2, "Good", 2010);

        // Act
        var property = Property.Create(branchId, "House", address, specifications, "Beautiful house");

        // Assert
        Assert.NotNull(property);
        Assert.Equal(branchId, property.BranchId);
        Assert.Equal("House", property.PropertyType);
        Assert.Equal(PropertyStatus.Registered, property.Status);
        Assert.Equal("Beautiful house", property.Description);
        Assert.Single(property.DomainEvents);
        Assert.IsType<PropertyRegisteredEvent>(property.DomainEvents.First());
    }

    [Fact]
    public void Create_ShouldRaisePropertyRegisteredEvent()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var specifications = new PropertySpecifications(2000m, 3, 2, "Good", 2010);

        // Act
        var property = Property.Create(branchId, "House", address, specifications);

        // Assert
        var @event = property.DomainEvents.First() as PropertyRegisteredEvent;
        Assert.NotNull(@event);
        Assert.Equal(property.Id, @event.PropertyId);
        Assert.Equal(branchId, @event.BranchId);
        Assert.Equal("House", @event.PropertyType);
        Assert.Equal(2000m, @event.Size);
        Assert.Equal(3, @event.Bedrooms);
        Assert.Equal("New York", @event.City);
        Assert.Equal("NY", @event.State);
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdateListedPrice()
    {
        // Arrange
        var property = CreateTestProperty();
        var newPrice = 500000m;

        // Act
        property.UpdatePrice(newPrice);

        // Assert
        Assert.Equal(newPrice, property.ListedPrice);
        Assert.NotNull(property.UpdatedAt);
    }

    [Fact]
    public void UpdatePrice_WithZeroPrice_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty();

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleValidationException>(() => property.UpdatePrice(0));
        Assert.Contains("Property price must be greater than zero", exception.Message);
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty();

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleValidationException>(() => property.UpdatePrice(-1000));
        Assert.Contains("Property price must be greater than zero", exception.Message);
    }

    [Fact]
    public void SetSuggestedPrice_WithValidPrice_ShouldUpdateSuggestedPrice()
    {
        // Arrange
        var property = CreateTestProperty();
        var suggestedPrice = 450000m;

        // Act
        property.SetSuggestedPrice(suggestedPrice);

        // Assert
        Assert.Equal(suggestedPrice, property.SuggestedPrice);
        Assert.NotNull(property.UpdatedAt);
    }

    [Fact]
    public void SetSuggestedPrice_WithZeroPrice_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => property.SetSuggestedPrice(0));
        Assert.Equal("Suggested price must be greater than zero (Parameter 'suggestedPrice')", exception.Message);
    }

    [Fact]
    public void SetSuggestedPrice_WithNegativePrice_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => property.SetSuggestedPrice(-1000));
        Assert.Equal("Suggested price must be greater than zero (Parameter 'suggestedPrice')", exception.Message);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatusAndRaiseEvent()
    {
        // Arrange
        var property = CreateTestProperty();
        var initialEventCount = property.DomainEvents.Count;

        // Act
        property.ChangeStatus(PropertyStatus.Available);

        // Assert
        Assert.Equal(PropertyStatus.Available, property.Status);
        Assert.NotNull(property.UpdatedAt);
        Assert.Equal(initialEventCount + 1, property.DomainEvents.Count);
        
        var statusEvent = property.DomainEvents.Last() as PropertyStatusChangedEvent;
        Assert.NotNull(statusEvent);
        Assert.Equal(property.Id, statusEvent.PropertyId);
        Assert.Equal(PropertyStatus.Registered.ToString(), statusEvent.OldStatus);
        Assert.Equal(PropertyStatus.Available.ToString(), statusEvent.NewStatus);
    }

    [Fact]
    public void ChangeStatus_ToSameStatus_ShouldNotRaiseEvent()
    {
        // Arrange
        var property = CreateTestProperty();
        var initialEventCount = property.DomainEvents.Count;

        // Act
        property.ChangeStatus(PropertyStatus.Registered);

        // Assert
        Assert.Equal(PropertyStatus.Registered, property.Status);
        Assert.Equal(initialEventCount, property.DomainEvents.Count);
    }

    [Fact]
    public void Reserve_WhenAvailable_ShouldChangeStatusToReserved()
    {
        // Arrange
        var property = CreateTestProperty();
        property.ChangeStatus(PropertyStatus.Available);

        // Act
        property.Reserve();

        // Assert
        Assert.Equal(PropertyStatus.Reserved, property.Status);
    }

    [Fact]
    public void Reserve_WhenNotAvailable_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty(); // Status is Registered

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => property.Reserve());
        Assert.Contains("Cannot reserve property with status Registered", exception.Message);
    }

    [Fact]
    public void MarkAsSold_WhenAvailable_ShouldChangeStatusToSold()
    {
        // Arrange
        var property = CreateTestProperty();
        property.ChangeStatus(PropertyStatus.Available);

        // Act
        property.MarkAsSold();

        // Assert
        Assert.Equal(PropertyStatus.Sold, property.Status);
    }

    [Fact]
    public void MarkAsSold_WhenReserved_ShouldChangeStatusToSold()
    {
        // Arrange
        var property = CreateTestProperty();
        property.ChangeStatus(PropertyStatus.Available);
        property.Reserve();

        // Act
        property.MarkAsSold();

        // Assert
        Assert.Equal(PropertyStatus.Sold, property.Status);
    }

    [Fact]
    public void MarkAsSold_WhenNotAvailableOrReserved_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty(); // Status is Registered

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => property.MarkAsSold());
        Assert.Contains("Cannot mark property as sold with status Registered", exception.Message);
    }

    [Fact]
    public void MakeAvailable_WhenPendingApproval_ShouldChangeStatusToAvailable()
    {
        // Arrange
        var property = CreateTestProperty();
        property.ChangeStatus(PropertyStatus.PendingApproval);

        // Act
        property.MakeAvailable();

        // Assert
        Assert.Equal(PropertyStatus.Available, property.Status);
    }

    [Fact]
    public void MakeAvailable_WhenReserved_ShouldChangeStatusToAvailable()
    {
        // Arrange
        var property = CreateTestProperty();
        property.ChangeStatus(PropertyStatus.Available);
        property.Reserve();

        // Act
        property.MakeAvailable();

        // Assert
        Assert.Equal(PropertyStatus.Available, property.Status);
    }

    [Fact]
    public void MakeAvailable_WhenRegistered_ShouldThrowException()
    {
        // Arrange
        var property = CreateTestProperty(); // Status is Registered

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => property.MakeAvailable());
        Assert.Contains("Cannot make property available with status Registered", exception.Message);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        // Arrange
        var property = CreateTestProperty();
        var newDescription = "Updated description";

        // Act
        property.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, property.Description);
        Assert.NotNull(property.UpdatedAt);
    }

    private Property CreateTestProperty()
    {
        var branchId = Guid.NewGuid();
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var specifications = new PropertySpecifications(2000m, 3, 2, "Good", 2010);
        return Property.Create(branchId, "House", address, specifications);
    }
}

