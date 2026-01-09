using Shared.Domain.Rules;

namespace PropertyManagement.Domain.Rules;

public class PropertyMustHaveValidPriceRule : IBusinessRule
{
    private readonly decimal? _price;

    public PropertyMustHaveValidPriceRule(decimal? price)
    {
        _price = price;
    }

    public bool IsBroken()
    {
        return _price.HasValue && _price.Value <= 0;
    }

    public string Message => "Property price must be greater than zero if provided";
}

