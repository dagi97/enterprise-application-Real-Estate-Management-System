using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Act
        var money = new Money(1000m, "USD");

        // Assert
        Assert.Equal(1000m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Create_WithDefaultCurrency_ShouldUseUSD()
    {
        // Act
        var money = new Money(1000m);

        // Assert
        Assert.Equal(1000m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreateMoney()
    {
        // Act
        var money = new Money(0m, "USD");

        // Assert
        Assert.Equal(0m, money.Amount);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(-1000m, "USD"));
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(1000m, ""));
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(500m, "USD");

        // Act
        var result = money1 + money2;

        // Assert
        Assert.Equal(1500m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(500m, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1 + money2);
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(300m, "USD");

        // Act
        var result = money1 - money2;

        // Assert
        Assert.Equal(700m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(500m, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1 - money2);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(1000m, "USD");

        // Act & Assert
        Assert.Equal(money1, money2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(1000m, "USD");
        var money2 = new Money(2000m, "USD");

        // Act & Assert
        Assert.NotEqual(money1, money2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(1000.50m, "USD");

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("1000.50 USD", result);
    }
}

public class CustomerInfoTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCustomerInfo()
    {
        // Act
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890", "123 Main St");

        // Assert
        Assert.Equal("John", customer.FirstName);
        Assert.Equal("Doe", customer.LastName);
        Assert.Equal("john@example.com", customer.Email);
        Assert.Equal("123-456-7890", customer.PhoneNumber);
        Assert.Equal("123 Main St", customer.Address);
        Assert.Equal("John Doe", customer.FullName);
    }

    [Fact]
    public void Create_WithoutAddress_ShouldCreateCustomerInfo()
    {
        // Act
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");

        // Assert
        Assert.Null(customer.Address);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo("", "Doe", "john@example.com", "123-456-7890"));
    }

    [Fact]
    public void Create_WithEmptyLastName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo("John", "", "john@example.com", "123-456-7890"));
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo("John", "Doe", "", "123-456-7890"));
    }

    [Fact]
    public void Create_WithEmptyPhoneNumber_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo("John", "Doe", "john@example.com", ""));
    }

    [Fact]
    public void FullName_ShouldReturnConcatenatedName()
    {
        // Arrange
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");

        // Assert
        Assert.Equal("John Doe", customer.FullName);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var customer1 = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890", "123 Main St");
        var customer2 = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890", "123 Main St");

        // Act & Assert
        Assert.Equal(customer1, customer2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var customer1 = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var customer2 = new CustomerInfo("Jane", "Doe", "john@example.com", "123-456-7890");

        // Act & Assert
        Assert.NotEqual(customer1, customer2);
    }
}

