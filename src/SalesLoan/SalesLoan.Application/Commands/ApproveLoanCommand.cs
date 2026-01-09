using MediatR;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Application.Commands;

public record ApproveLoanCommand(
    Guid LoanApplicationId,
    decimal ApprovedAmount,
    string Currency = "USD") : IRequest;

