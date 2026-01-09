using MediatR;
using SalesLoan.Application.DTOs;

namespace SalesLoan.Application.Queries;

public record GetLoanApplicationQuery(Guid LoanApplicationId) : IRequest<LoanApplicationDto?>;

