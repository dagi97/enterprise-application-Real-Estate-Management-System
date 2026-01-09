using PropertyManagement.Domain.Rules;

namespace PropertyManagement.Domain.Tests;

public class PropertyMustHaveValidPriceRuleTests
{
    [Fact]
    public void IsBroken_WithPositivePrice_ShouldReturnFalse()
    {
        // Arrange
        var rule = new PropertyMustHaveValidPriceRule(500000m);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBroken_WithZeroPrice_ShouldReturnTrue()
    {
        // Arrange
        var rule = new PropertyMustHaveValidPriceRule(0);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBroken_WithNegativePrice_ShouldReturnTrue()
    {
        // Arrange
        var rule = new PropertyMustHaveValidPriceRule(-1000m);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBroken_WithNullPrice_ShouldReturnFalse()
    {
        // Arrange
        var rule = new PropertyMustHaveValidPriceRule(null);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Message_ShouldReturnCorrectMessage()
    {
        // Arrange
        var rule = new PropertyMustHaveValidPriceRule(0);

        // Assert
        Assert.Equal("Property price must be greater than zero if provided", rule.Message);
    }
}

