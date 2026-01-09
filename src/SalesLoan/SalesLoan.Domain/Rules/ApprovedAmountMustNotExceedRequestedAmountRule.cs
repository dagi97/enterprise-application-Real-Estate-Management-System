using Shared.Domain.Rules;

namespace SalesLoan.Domain.Rules;

public class ApprovedAmountMustNotExceedRequestedAmountRule : IBusinessRule
{
    private readonly decimal _requestedAmount;
    private readonly decimal _approvedAmount;

    public ApprovedAmountMustNotExceedRequestedAmountRule(decimal requestedAmount, decimal approvedAmount)
    {
        _requestedAmount = requestedAmount;
        _approvedAmount = approvedAmount;
    }

    public bool IsBroken()
    {
        return _approvedAmount > _requestedAmount;
    }

    public string Message => "Approved amount cannot exceed requested amount";
}

