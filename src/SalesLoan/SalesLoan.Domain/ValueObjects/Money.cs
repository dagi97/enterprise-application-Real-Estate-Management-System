using Shared.Domain.ValueObjects;

namespace SalesLoan.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}

