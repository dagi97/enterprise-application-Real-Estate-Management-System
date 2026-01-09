using MediatR;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Application.Commands;

public record CreateLoanApplicationCommand(
    Guid PropertyId,
    Guid BranchId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? Address,
    decimal RequestedAmount,
    string Currency = "USD") : IRequest<Guid>;

