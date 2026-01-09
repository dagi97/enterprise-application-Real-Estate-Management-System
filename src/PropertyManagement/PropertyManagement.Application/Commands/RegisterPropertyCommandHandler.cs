using MediatR;
using PropertyManagement.Application.Commands;
using PropertyManagement.Domain.Aggregates;
using PropertyManagement.Domain.Repositories;
using PropertyManagement.Domain.ValueObjects;

namespace PropertyManagement.Application.Commands;

public class RegisterPropertyCommandHandler : IRequestHandler<RegisterPropertyCommand, Guid>
{
    private readonly IPropertyRepository _propertyRepository;

    public RegisterPropertyCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Guid> Handle(RegisterPropertyCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country);

        var specifications = new PropertySpecifications(
            request.Size,
            request.Bedrooms,
            request.Bathrooms,
            request.Condition,
            request.YearBuilt);

        var property = Property.Create(
            request.BranchId,
            request.PropertyType,
            address,
            specifications,
            request.Description);

        await _propertyRepository.AddAsync(property, cancellationToken);

        return property.Id;
    }
}

