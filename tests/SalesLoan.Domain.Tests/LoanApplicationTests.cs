using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Enums;
using SalesLoan.Domain.Events;
using SalesLoan.Domain.Rules;
using SalesLoan.Domain.ValueObjects;
using Shared.Domain.Rules;

namespace SalesLoan.Domain.Tests;

public class LoanApplicationTests
{
    [Fact]
    public void Create_ShouldCreateLoanApplicationWithPendingStatus()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var requestedAmount = new Money(400000m, "USD");

        // Act
        var application = LoanApplication.Create(propertyId, branchId, customer, requestedAmount);

        // Assert
        Assert.NotNull(application);
        Assert.Equal(propertyId, application.PropertyId);
        Assert.Equal(branchId, application.BranchId);
        Assert.Equal(customer, application.Customer);
        Assert.Equal(requestedAmount, application.RequestedAmount);
        Assert.Equal(LoanStatus.Pending, application.Status);
        Assert.False(application.IsPropertyReserved);
        Assert.Null(application.ApprovedAmount);
        Assert.Null(application.RejectionReason);
    }

    [Fact]
    public void Create_ShouldRaiseLoanRequestedEvent()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var requestedAmount = new Money(400000m, "USD");

        // Act
        var application = LoanApplication.Create(propertyId, branchId, customer, requestedAmount);

        // Assert
        Assert.Single(application.DomainEvents);
        var @event = application.DomainEvents.First() as LoanRequestedEvent;
        Assert.NotNull(@event);
        Assert.Equal(application.Id, @event.LoanApplicationId);
        Assert.Equal(propertyId, @event.PropertyId);
        Assert.Equal(branchId, @event.BranchId);
        Assert.Equal("john@example.com", @event.CustomerEmail);
        Assert.Equal(400000m, @event.LoanAmount);
    }

    [Fact]
    public void Approve_WithValidAmount_ShouldApproveLoan()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        var approvedAmount = new Money(400000m, "USD");

        // Act
        application.Approve(approvedAmount);

        // Assert
        Assert.Equal(LoanStatus.Approved, application.Status);
        Assert.Equal(approvedAmount, application.ApprovedAmount);
        Assert.NotNull(application.ReviewedAt);
        Assert.NotNull(application.ApprovedAt);
        Assert.Equal(2, application.DomainEvents.Count);
        
        var approveEvent = application.DomainEvents.Last() as LoanApprovedEvent;
        Assert.NotNull(approveEvent);
        Assert.Equal(application.Id, approveEvent.LoanApplicationId);
        Assert.Equal(400000m, approveEvent.ApprovedAmount);
    }

    [Fact]
    public void Approve_WithAmountLessThanRequested_ShouldApproveLoan()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        var approvedAmount = new Money(350000m, "USD"); // Less than requested

        // Act
        application.Approve(approvedAmount);

        // Assert
        Assert.Equal(LoanStatus.Approved, application.Status);
        Assert.Equal(approvedAmount, application.ApprovedAmount);
    }

    [Fact]
    public void Approve_WithAmountEqualToRequested_ShouldApproveLoan()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        var approvedAmount = new Money(400000m, "USD"); // Equal to requested

        // Act
        application.Approve(approvedAmount);

        // Assert
        Assert.Equal(LoanStatus.Approved, application.Status);
    }

    [Fact]
    public void Approve_WithAmountExceedingRequested_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        var approvedAmount = new Money(500000m, "USD"); // More than requested

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleValidationException>(() => application.Approve(approvedAmount));
        Assert.Contains("Approved amount cannot exceed requested amount", exception.Message);
    }

    [Fact]
    public void Approve_WhenUnderReview_ShouldApproveLoan()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.MarkAsUnderReview();
        var approvedAmount = new Money(400000m, "USD");

        // Act
        application.Approve(approvedAmount);

        // Assert
        Assert.Equal(LoanStatus.Approved, application.Status);
    }

    [Fact]
    public void Approve_WhenNotPendingOrUnderReview_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));
        var approvedAmount = new Money(400000m, "USD");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.Approve(approvedAmount));
        Assert.Contains("Cannot approve loan with status Approved", exception.Message);
    }

    [Fact]
    public void Reject_WithValidReason_ShouldRejectLoan()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        var reason = "Insufficient credit score";

        // Act
        application.Reject(reason);

        // Assert
        Assert.Equal(LoanStatus.Rejected, application.Status);
        Assert.Equal(reason, application.RejectionReason);
        Assert.NotNull(application.ReviewedAt);
    }

    [Fact]
    public void Reject_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => application.Reject(""));
        Assert.Contains("Rejection reason cannot be empty", exception.Message);
    }

    [Fact]
    public void Reject_WhenNotPendingOrUnderReview_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.Reject("Reason"));
        Assert.Contains("Cannot reject loan with status Approved", exception.Message);
    }

    [Fact]
    public void MarkAsUnderReview_WhenPending_ShouldChangeStatus()
    {
        // Arrange
        var application = CreateTestLoanApplication();

        // Act
        application.MarkAsUnderReview();

        // Assert
        Assert.Equal(LoanStatus.UnderReview, application.Status);
    }

    [Fact]
    public void MarkAsUnderReview_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.MarkAsUnderReview();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.MarkAsUnderReview());
        Assert.Contains("Cannot mark as under review with status UnderReview", exception.Message);
    }

    [Fact]
    public void ReserveProperty_WhenApproved_ShouldReserveProperty()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));

        // Act
        application.ReserveProperty();

        // Assert
        Assert.True(application.IsPropertyReserved);
        Assert.Equal(3, application.DomainEvents.Count);
        
        var reserveEvent = application.DomainEvents.Last() as PropertyReservedEvent;
        Assert.NotNull(reserveEvent);
        Assert.Equal(application.PropertyId, reserveEvent.PropertyId);
        Assert.Equal(application.BranchId, reserveEvent.BranchId);
        Assert.Equal(application.Id, reserveEvent.LoanApplicationId);
    }

    [Fact]
    public void ReserveProperty_WhenAlreadyReserved_ShouldNotThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));
        application.ReserveProperty();

        // Act & Assert
        application.ReserveProperty(); // Should not throw
        Assert.True(application.IsPropertyReserved);
    }

    [Fact]
    public void ReserveProperty_WhenNotApproved_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.ReserveProperty());
        Assert.Contains("Property can only be reserved for approved loans", exception.Message);
    }

    [Fact]
    public void MarkAsDisbursed_WhenApproved_ShouldChangeStatus()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));

        // Act
        application.MarkAsDisbursed();

        // Assert
        Assert.Equal(LoanStatus.Disbursed, application.Status);
    }

    [Fact]
    public void MarkAsDisbursed_WhenNotApproved_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.MarkAsDisbursed());
        Assert.Contains("Cannot disburse loan with status Pending", exception.Message);
    }

    [Fact]
    public void Close_WhenDisbursed_ShouldChangeStatus()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));
        application.MarkAsDisbursed();

        // Act
        application.Close();

        // Assert
        Assert.Equal(LoanStatus.Closed, application.Status);
    }

    [Fact]
    public void Close_WhenNotDisbursed_ShouldThrowException()
    {
        // Arrange
        var application = CreateTestLoanApplication();
        application.Approve(new Money(400000m, "USD"));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => application.Close());
        Assert.Contains("Cannot close loan with status Approved", exception.Message);
    }

    private LoanApplication CreateTestLoanApplication()
    {
        var propertyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customer = new CustomerInfo("John", "Doe", "john@example.com", "123-456-7890");
        var requestedAmount = new Money(400000m, "USD");
        return LoanApplication.Create(propertyId, branchId, customer, requestedAmount);
    }
}

