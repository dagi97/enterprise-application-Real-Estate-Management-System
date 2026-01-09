# PropertyManagement Bounded Context - Class Diagram

## Overview
This bounded context manages property registration, pricing, and status tracking.

## Class Diagram

```mermaid
classDiagram
    class Property {
        -Guid Id
        -Guid BranchId
        -string PropertyType
        -Address Address
        -PropertySpecifications Specifications
        -PropertyStatus Status
        -decimal? ListedPrice
        -decimal? SuggestedPrice
        -string? Description
        -DateTime RegisteredAt
        -DateTime? UpdatedAt
        +Create(branchId, propertyType, address, specifications, description) Property
        +UpdatePrice(price) void
        +SetSuggestedPrice(suggestedPrice) void
        +ChangeStatus(newStatus) void
        +UpdateDescription(description) void
        +Reserve() void
        +MarkAsSold() void
        +MakeAvailable() void
    }

    class Address {
        -string Street
        -string City
        -string State
        -string ZipCode
        -string Country
        +Address(street, city, state, zipCode, country)
        +ToString() string
    }

    class PropertySpecifications {
        -decimal Size
        -int Bedrooms
        -int Bathrooms
        -string Condition
        -int? YearBuilt
        +PropertySpecifications(size, bedrooms, bathrooms, condition, yearBuilt)
    }

    class PropertyStatus {
        <<enumeration>>
        Draft
        Registered
        PendingApproval
        Available
        Reserved
        Sold
        Withdrawn
    }

    class PropertyRegisteredEvent {
        +Guid PropertyId
        +Guid BranchId
        +string PropertyType
        +decimal Size
        +int Bedrooms
        +string City
        +string State
    }

    class PropertyStatusChangedEvent {
        +Guid PropertyId
        +string OldStatus
        +string NewStatus
    }

    class PropertyMustHaveValidPriceRule {
        -decimal? _price
        +IsBroken() bool
        +string Message
    }

    class IPropertyRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Property~
        +AddAsync(property) Task
        +UpdateAsync(property) Task
        +SaveDomainEventsAsync(property) Task
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

    class IBusinessRule {
        <<interface>>
        +IsBroken() bool
        +string Message
    }

    Property --|> AggregateRoot~Guid~
    AggregateRoot~Guid~ --|> Entity~Guid~
    Property *-- Address : contains
    Property *-- PropertySpecifications : contains
    Property --> PropertyStatus : uses
    Property ..> PropertyRegisteredEvent : raises
    Property ..> PropertyStatusChangedEvent : raises
    Property ..> PropertyMustHaveValidPriceRule : validates
    Address --|> ValueObject
    PropertySpecifications --|> ValueObject
    PropertyRegisteredEvent --|> DomainEvent
    PropertyStatusChangedEvent --|> DomainEvent
    PropertyMustHaveValidPriceRule ..|> IBusinessRule
    IPropertyRepository ..> Property : manages
```

## Key Components

### Aggregates
- **Property**: Main aggregate root that manages property lifecycle, pricing, and status transitions

### Value Objects
- **Address**: Immutable address value object with validation
- **PropertySpecifications**: Property specifications (size, bedrooms, bathrooms, condition, year built)

### Enums
- **PropertyStatus**: Represents the current state of a property (Draft, Registered, Available, Reserved, Sold, etc.)

### Domain Events
- **PropertyRegisteredEvent**: Raised when a property is first registered
- **PropertyStatusChangedEvent**: Raised when property status changes

### Business Rules
- **PropertyMustHaveValidPriceRule**: Ensures property prices are greater than zero

### Repository
- **IPropertyRepository**: Repository interface for property persistence

## Relationships

1. **Property** is the aggregate root that contains **Address** and **PropertySpecifications** as value objects
2. **Property** raises domain events when significant state changes occur
3. **Property** uses business rules to validate operations (e.g., price updates)
4. **IPropertyRepository** manages persistence of **Property** aggregates

