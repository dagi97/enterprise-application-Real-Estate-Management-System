# Real Estate Management System - Documentation

This folder contains the documentation deliverables for the Real Estate Management System project.

## Class Diagrams

### PropertyManagement Bounded Context
- **File**: [PropertyManagement-ClassDiagram.md](./PropertyManagement-ClassDiagram.md)
- **Description**: Class diagram showing the Property aggregate, value objects (Address, PropertySpecifications), domain events, and business rules for property management.

### PricingValuation Bounded Context
- **File**: [PricingValuation-ClassDiagram.md](./PricingValuation-ClassDiagram.md)
- **Description**: Class diagram showing the PriceEstimate aggregate, PricingFactors value object, and domain services for price estimation.

### SalesLoan Bounded Context
- **File**: [SalesLoan-ClassDiagram.md](./SalesLoan-ClassDiagram.md)
- **Description**: Class diagram showing LoanApplication and Sale aggregates, value objects (CustomerInfo, Money), domain events, and business rules for loan and sales management.

## Outbox Implementation

### Transactional Outbox Pattern
- **File**: [Outbox-Implementation.md](./Outbox-Implementation.md)
- **Description**: Comprehensive documentation explaining:
  - Architecture and flow diagrams
  - Component details (OutboxMessage, OutboxPublisher, Quartz Job)
  - Implementation flow with code examples
  - Retry logic and error handling
  - Database schema
  - Configuration
  - Monitoring queries
  - Troubleshooting guide

## Viewing the Diagrams

The class diagrams are written in Mermaid format, which can be viewed:

1. **GitHub**: Automatically renders Mermaid diagrams in markdown files
2. **VS Code**: Install the "Markdown Preview Mermaid Support" extension
3. **Online**: Copy the Mermaid code to [mermaid.live](https://mermaid.live)
4. **Documentation Tools**: Most modern documentation platforms support Mermaid

## Project Structure

```
realestate_management/
├── docs/                          # Documentation (this folder)
│   ├── PropertyManagement-ClassDiagram.md
│   ├── PricingValuation-ClassDiagram.md
│   ├── SalesLoan-ClassDiagram.md
│   ├── Outbox-Implementation.md
│   └── README.md
├── src/
│   ├── PropertyManagement/        # Property Management Bounded Context
│   ├── PricingValuation/         # Pricing & Valuation Bounded Context
│   ├── SalesLoan/                 # Sales & Loan Bounded Context
│   └── Shared/                    # Shared Kernel
├── docker/                        # Docker Compose configuration
└── tests/                         # Unit tests
```

## Related Documentation

- **Main README**: [../README.md](../README.md) - Project overview and setup
- **Testing Guide**: [../TESTING_GUIDE.md](../TESTING_GUIDE.md) - API testing instructions
- **Outbox Implementation (Original)**: [../OUTBOX_IMPLEMENTATION.md](../OUTBOX_IMPLEMENTATION.md) - Original outbox documentation

