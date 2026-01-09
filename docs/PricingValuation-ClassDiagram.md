# PricingValuation Bounded Context - Class Diagram

## Overview
This bounded context handles property price estimation using various factors and pricing models.

## Class Diagram

```mermaid
classDiagram
    class PriceEstimate {
        -Guid Id
        -Guid PropertyId
        -decimal EstimatedPrice
        -decimal ConfidenceScore
        -PricingFactors Factors
        -string ModelVersion
        -DateTime EstimatedAt
        -bool IsApplied
        +Create(propertyId, estimatedPrice, confidenceScore, factors, modelVersion) PriceEstimate
        +MarkAsApplied() void
    }

    class PricingFactors {
        -decimal SizeFactor
        -decimal LocationFactor
        -decimal ConditionFactor
        -decimal BedroomFactor
        -decimal YearBuiltFactor
        +PricingFactors(sizeFactor, locationFactor, conditionFactor, bedroomFactor, yearBuiltFactor)
    }

    class PriceSuggestedEvent {
        +Guid PropertyId
        +decimal SuggestedPrice
        +decimal ConfidenceScore
        +string ModelVersion
    }

    class IPriceEstimationService {
        <<interface>>
        +EstimatePriceAsync(size, bedrooms, bathrooms, condition, city, state, yearBuilt, cancellationToken) Task~decimal~
    }

    class IPriceEstimateRepository {
        <<interface>>
        +GetByPropertyIdAsync(propertyId) Task~PriceEstimate~
        +AddAsync(estimate) Task
        +UpdateAsync(estimate) Task
        +SaveDomainEventsAsync(estimate) Task
    }

    class AggregateRoot~TId~ {
        <<abstract>>
        +IReadOnlyCollection~DomainEvent~ DomainEvents
        #RaiseDomainEvent(event) void
        +ClearDomainEvents() void
        #CheckRule(rule) void
    }

    class Entity~TId~ {
        <<abstract>>
        +TId Id
    }

    class ValueObject {
        <<abstract>>
        #GetEqualityComponents() IEnumerable~object~
    }

    class DomainEvent {
        <<abstract>>
        +Guid Id
        +DateTime OccurredOn
    }

    PriceEstimate --|> AggregateRoot~Guid~
    AggregateRoot~Guid~ --|> Entity~Guid~
    PriceEstimate *-- PricingFactors : contains
    PriceEstimate ..> PriceSuggestedEvent : raises
    PricingFactors --|> ValueObject
    PriceSuggestedEvent --|> DomainEvent
    IPriceEstimationService ..> PriceEstimate : creates
    IPriceEstimateRepository ..> PriceEstimate : manages
```

## Key Components

### Aggregates
- **PriceEstimate**: Aggregate root that represents a price estimation for a property with confidence scoring

### Value Objects
- **PricingFactors**: Immutable value object containing all factors used in price calculation (size, location, condition, bedrooms, year built)

### Domain Events
- **PriceSuggestedEvent**: Raised when a price estimate is created, containing the suggested price and confidence score

### Domain Services
- **IPriceEstimationService**: Service interface for price estimation logic (implemented in Infrastructure layer)

### Repository
- **IPriceEstimateRepository**: Repository interface for price estimate persistence

## Relationships

1. **PriceEstimate** is the aggregate root that contains **PricingFactors** as a value object
2. **PriceEstimate** raises **PriceSuggestedEvent** when created
3. **IPriceEstimationService** provides the business logic for calculating price estimates
4. **IPriceEstimateRepository** manages persistence of **PriceEstimate** aggregates

## Business Flow

1. When a **PropertyRegisteredEvent** is received, the pricing service estimates the price
2. A **PriceEstimate** aggregate is created with pricing factors
3. **PriceSuggestedEvent** is raised and published
4. The PropertyManagement context receives the event and updates the property's suggested price

