using MediatR;
using PropertyManagement.Application.DTOs;
using PropertyManagement.Application.Queries;
using PropertyManagement.Domain.Repositories;

namespace PropertyManagement.Application.Queries;

public class GetPropertyQueryHandler : IRequestHandler<GetPropertyQuery, PropertyDto?>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto?> Handle(GetPropertyQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        
        if (property == null)
            return null;

        return new PropertyDto
        {
            Id = property.Id,
            BranchId = property.BranchId,
            PropertyType = property.PropertyType,
            Street = property.Address.Street,
            City = property.Address.City,
            State = property.Address.State,
            ZipCode = property.Address.ZipCode,
            Country = property.Address.Country,
            Size = property.Specifications.Size,
            Bedrooms = property.Specifications.Bedrooms,
            Bathrooms = property.Specifications.Bathrooms,
            Condition = property.Specifications.Condition,
            YearBuilt = property.Specifications.YearBuilt,
            Status = property.Status.ToString(),
            ListedPrice = property.ListedPrice,
            SuggestedPrice = property.SuggestedPrice,
            Description = property.Description,
            RegisteredAt = property.RegisteredAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}

