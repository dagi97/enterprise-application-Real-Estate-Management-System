using MediatR;
using SalesLoan.Application.DTOs;
using SalesLoan.Application.Queries;
using SalesLoan.Domain.Repositories;

namespace SalesLoan.Application.Queries;

public class GetLoanApplicationQueryHandler : IRequestHandler<GetLoanApplicationQuery, LoanApplicationDto?>
{
    private readonly ILoanApplicationRepository _loanApplicationRepository;

    public GetLoanApplicationQueryHandler(ILoanApplicationRepository loanApplicationRepository)
    {
        _loanApplicationRepository = loanApplicationRepository;
    }

    public async Task<LoanApplicationDto?> Handle(GetLoanApplicationQuery request, CancellationToken cancellationToken)
    {
        var loanApplication = await _loanApplicationRepository.GetByIdAsync(request.LoanApplicationId, cancellationToken);
        
        if (loanApplication == null)
            return null;

        return new LoanApplicationDto
        {
            Id = loanApplication.Id,
            PropertyId = loanApplication.PropertyId,
            BranchId = loanApplication.BranchId,
            CustomerFirstName = loanApplication.Customer.FirstName,
            CustomerLastName = loanApplication.Customer.LastName,
            CustomerEmail = loanApplication.Customer.Email,
            CustomerPhoneNumber = loanApplication.Customer.PhoneNumber,
            RequestedAmount = loanApplication.RequestedAmount.Amount,
            Currency = loanApplication.RequestedAmount.Currency,
            ApprovedAmount = loanApplication.ApprovedAmount?.Amount,
            Status = loanApplication.Status.ToString(),
            RejectionReason = loanApplication.RejectionReason,
            AppliedAt = loanApplication.AppliedAt,
            IsPropertyReserved = loanApplication.IsPropertyReserved
        };
    }
}

