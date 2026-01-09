using MediatR;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Application.Commands;

public record CreateSaleCommand(
    Guid PropertyId,
    Guid BranchId,
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? Address,
    decimal SaleAmount,
    string Currency = "USD",
    string? Notes = null) : IRequest<Guid>;

