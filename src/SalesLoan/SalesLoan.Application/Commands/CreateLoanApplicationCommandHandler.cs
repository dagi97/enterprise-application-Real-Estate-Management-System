using MediatR;
using SalesLoan.Application.Commands;
using SalesLoan.Domain.Aggregates;
using SalesLoan.Domain.Repositories;
using SalesLoan.Domain.ValueObjects;

namespace SalesLoan.Application.Commands;

public class CreateLoanApplicationCommandHandler : IRequestHandler<CreateLoanApplicationCommand, Guid>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;

    public CreateLoanApplicationCommandHandler(ILoanApplicationRepository loanApplicationRepository)
    {
        _loanApplicationRepository = loanApplicationRepository;
    }

    public async Task<Guid> Handle(CreateLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var customer = new CustomerInfo(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Address);

        var requestedAmount = new Money(request.RequestedAmount, request.Currency);

        var loanApplication = LoanApplication.Create(
            request.PropertyId,
            request.BranchId,
            customer,
            requestedAmount);

        await _loanApplicationRepository.AddAsync(loanApplication, cancellationToken);

        return loanApplication.Id;
    }
}

