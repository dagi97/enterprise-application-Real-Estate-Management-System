using PricingValuation.Domain.Aggregates;
using PricingValuation.Domain.Events;
using PricingValuation.Domain.ValueObjects;

namespace PricingValuation.Domain.Tests;

public class PriceEstimateTests
{
    [Fact]
    public void Create_ShouldCreatePriceEstimateWithCorrectProperties()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var estimatedPrice = 450000m;
        var confidenceScore = 0.85m;
        var factors = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);
        var modelVersion = "v1.0";

        // Act
        var estimate = PriceEstimate.Create(propertyId, estimatedPrice, confidenceScore, factors, modelVersion);

        // Assert
        Assert.NotNull(estimate);
        Assert.Equal(propertyId, estimate.PropertyId);
        Assert.Equal(estimatedPrice, estimate.EstimatedPrice);
        Assert.Equal(confidenceScore, estimate.ConfidenceScore);
        Assert.Equal(factors, estimate.Factors);
        Assert.Equal(modelVersion, estimate.ModelVersion);
        Assert.False(estimate.IsApplied);
    }

    [Fact]
    public void Create_ShouldRaisePriceSuggestedEvent()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var estimatedPrice = 450000m;
        var confidenceScore = 0.85m;
        var factors = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);
        var modelVersion = "v1.0";

        // Act
        var estimate = PriceEstimate.Create(propertyId, estimatedPrice, confidenceScore, factors, modelVersion);

        // Assert
        Assert.Single(estimate.DomainEvents);
        var @event = estimate.DomainEvents.First() as PriceSuggestedEvent;
        Assert.NotNull(@event);
        Assert.Equal(propertyId, @event.PropertyId);
        Assert.Equal(estimatedPrice, @event.SuggestedPrice);
        Assert.Equal(confidenceScore, @event.ConfidenceScore);
        Assert.Equal(modelVersion, @event.ModelVersion);
    }

    [Fact]
    public void MarkAsApplied_WhenNotApplied_ShouldSetIsAppliedToTrue()
    {
        // Arrange
        var estimate = CreateTestPriceEstimate();

        // Act
        estimate.MarkAsApplied();

        // Assert
        Assert.True(estimate.IsApplied);
    }

    [Fact]
    public void MarkAsApplied_WhenAlreadyApplied_ShouldNotThrowException()
    {
        // Arrange
        var estimate = CreateTestPriceEstimate();
        estimate.MarkAsApplied();

        // Act & Assert
        estimate.MarkAsApplied(); // Should not throw
        Assert.True(estimate.IsApplied);
    }

    private PriceEstimate CreateTestPriceEstimate()
    {
        var propertyId = Guid.NewGuid();
        var estimatedPrice = 450000m;
        var confidenceScore = 0.85m;
        var factors = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);
        var modelVersion = "v1.0";
        return PriceEstimate.Create(propertyId, estimatedPrice, confidenceScore, factors, modelVersion);
    }
}

public class PricingFactorsTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePricingFactors()
    {
        // Act
        var factors = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);

        // Assert
        Assert.Equal(1.2m, factors.SizeFactor);
        Assert.Equal(1.1m, factors.LocationFactor);
        Assert.Equal(1.0m, factors.ConditionFactor);
        Assert.Equal(1.05m, factors.BedroomFactor);
        Assert.Equal(0.95m, factors.YearBuiltFactor);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var factors1 = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);
        var factors2 = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);

        // Act & Assert
        Assert.Equal(factors1, factors2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var factors1 = new PricingFactors(1.2m, 1.1m, 1.0m, 1.05m, 0.95m);
        var factors2 = new PricingFactors(1.3m, 1.1m, 1.0m, 1.05m, 0.95m);

        // Act & Assert
        Assert.NotEqual(factors1, factors2);
    }
}

