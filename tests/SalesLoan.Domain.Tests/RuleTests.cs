using SalesLoan.Domain.Rules;

namespace SalesLoan.Domain.Tests;

public class ApprovedAmountMustNotExceedRequestedAmountRuleTests
{
    [Fact]
    public void IsBroken_WithApprovedAmountLessThanRequested_ShouldReturnFalse()
    {
        // Arrange
        var rule = new ApprovedAmountMustNotExceedRequestedAmountRule(400000m, 350000m);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBroken_WithApprovedAmountEqualToRequested_ShouldReturnFalse()
    {
        // Arrange
        var rule = new ApprovedAmountMustNotExceedRequestedAmountRule(400000m, 400000m);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBroken_WithApprovedAmountGreaterThanRequested_ShouldReturnTrue()
    {
        // Arrange
        var rule = new ApprovedAmountMustNotExceedRequestedAmountRule(400000m, 500000m);

        // Act
        var result = rule.IsBroken();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Message_ShouldReturnCorrectMessage()
    {
        // Arrange
        var rule = new ApprovedAmountMustNotExceedRequestedAmountRule(400000m, 500000m);

        // Assert
        Assert.Equal("Approved amount cannot exceed requested amount", rule.Message);
    }
}

