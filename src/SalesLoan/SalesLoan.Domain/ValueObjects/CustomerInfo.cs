using Shared.Domain.ValueObjects;

namespace SalesLoan.Domain.ValueObjects;

public class CustomerInfo : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string? Address { get; }

    private CustomerInfo() { }

    public CustomerInfo(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return Email;
        yield return PhoneNumber;
        if (Address != null)
            yield return Address;
    }

    public string FullName => $"{FirstName} {LastName}";
}

