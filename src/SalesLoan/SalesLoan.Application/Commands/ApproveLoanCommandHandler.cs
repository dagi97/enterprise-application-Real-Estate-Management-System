using MediatR;
using SalesLoan.Application.Commands;
using SalesLoan.Domain.Repositories;

namespace SalesLoan.Application.Commands;

public class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;

    public ApproveLoanCommandHandler(ILoanApplicationRepository loanApplicationRepository)
    {
        _loanApplicationRepository = loanApplicationRepository;
    }

    public async Task Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _loanApplicationRepository.GetByIdAsync(request.LoanApplicationId, cancellationToken);
        
        if (loanApplication == null)
            throw new InvalidOperationException($"Loan application with ID {request.LoanApplicationId} not found");

        var approvedAmount = new SalesLoan.Domain.ValueObjects.Money(request.ApprovedAmount, request.Currency);
        loanApplication.Approve(approvedAmount);
        loanApplication.ReserveProperty();

        await _loanApplicationRepository.UpdateAsync(loanApplication, cancellationToken);
    }
}

