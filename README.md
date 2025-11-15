# Centralized Real Estate Management System

## Team Members

| Full Name       | ID          |
| --------------- | ----------- |
| Dagmawit Sisay  | UGR-2038-15 |
| Helina Tarekegn | UGR-9858-13 |
| Mekdes Assefa   | UGR-1419-15 |
| Meklit Shiferaw | UGR-5036-15 |
| Meklit Tadie    | UGR-3049-15 |
 

## Project Proposal

### Objective

Develop a centralized real estate system for multiple branches to register properties, process sales and loans, and report activities in real-time. The system will be designed using Domain-Driven Design (DDD) principles.

### Business Problem

- Inconsistent property pricing across branches
- Lack of real-time visibility into sales, loans, and branch performance
- Manual reporting causing delays and errors
- Difficulty monitoring and approving branch activities centrally

### Subdomains

**Core:** Branch Operations & Transaction Management  
**Supporting:** Customer Management, Branch Performance Tracking  
**Generic:** Authentication & Authorization, Notifications

### Core Bounded Contexts

- **Property Management:** Register/update properties, track status
- **Pricing & Valuation:** Suggest property prices via AI estimator
- **Sales & Loan:** Manage sales, customer transactions, and loan applications

### Key User Stories

1. **Branch Registers a Property** – Events: `PropertyRegistered`, `PriceSuggested`
2. **Customer Applies for a Loan** – Events: `LoanRequested`, `LoanApproved`, `PropertyReserved`
3. **Property is Sold** – Events: `PropertySold`, `BranchPerformanceUpdated`

### AI-Driven Feature

- Suggests fair market prices based on size, bedrooms, location, and condition
- Ensures consistent pricing and speeds up property approvals

### Summary

Centralized system enabling consistent pricing, efficient loan/sales processing, real-time monitoring, and modular design for future scalability.
