using PropertyManagement.Domain.ValueObjects;

namespace PropertyManagement.Domain.Tests;

public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAddress()
    {
        // Act
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");

        // Assert
        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("New York", address.City);
        Assert.Equal("NY", address.State);
        Assert.Equal("10001", address.ZipCode);
        Assert.Equal("USA", address.Country);
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("", "New York", "NY", "10001", "USA"));
    }

    [Fact]
    public void Create_WithEmptyCity_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("123 Main St", "", "NY", "10001", "USA"));
    }

    [Fact]
    public void Create_WithEmptyState_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("123 Main St", "New York", "", "10001", "USA"));
    }

    [Fact]
    public void Create_WithEmptyZipCode_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("123 Main St", "New York", "NY", "", "USA"));
    }

    [Fact]
    public void Create_WithEmptyCountry_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("123 Main St", "New York", "NY", "10001", ""));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedAddress()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");

        // Act
        var result = address.ToString();

        // Assert
        Assert.Equal("123 Main St, New York, NY 10001, USA", result);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var address2 = new Address("123 Main St", "New York", "NY", "10001", "USA");

        // Act & Assert
        Assert.Equal(address1, address2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var address1 = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var address2 = new Address("456 Oak Ave", "New York", "NY", "10001", "USA");

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }
}

public class PropertySpecificationsTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateSpecifications()
    {
        // Act
        var specs = new PropertySpecifications(2000m, 3, 2, "Good", 2010);

        // Assert
        Assert.Equal(2000m, specs.Size);
        Assert.Equal(3, specs.Bedrooms);
        Assert.Equal(2, specs.Bathrooms);
        Assert.Equal("Good", specs.Condition);
        Assert.Equal(2010, specs.YearBuilt);
    }

    [Fact]
    public void Create_WithZeroSize_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PropertySpecifications(0, 3, 2, "Good", 2010));
    }

    [Fact]
    public void Create_WithNegativeSize_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PropertySpecifications(-100, 3, 2, "Good", 2010));
    }

    [Fact]
    public void Create_WithNegativeBedrooms_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PropertySpecifications(2000m, -1, 2, "Good", 2010));
    }

    [Fact]
    public void Create_WithNegativeBathrooms_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PropertySpecifications(2000m, 3, -1, "Good", 2010));
    }

    [Fact]
    public void Create_WithEmptyCondition_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PropertySpecifications(2000m, 3, 2, "", 2010));
    }

    [Fact]
    public void Create_WithNullYearBuilt_ShouldCreateSpecifications()
    {
        // Act
        var specs = new PropertySpecifications(2000m, 3, 2, "Good", null);

        // Assert
        Assert.Null(specs.YearBuilt);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var specs1 = new PropertySpecifications(2000m, 3, 2, "Good", 2010);
        var specs2 = new PropertySpecifications(2000m, 3, 2, "Good", 2010);

        // Act & Assert
        Assert.Equal(specs1, specs2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var specs1 = new PropertySpecifications(2000m, 3, 2, "Good", 2010);
        var specs2 = new PropertySpecifications(3000m, 3, 2, "Good", 2010);

        // Act & Assert
        Assert.NotEqual(specs1, specs2);
    }
}

