using RealEstate.Property.Application.DTOs;
using PropertyAggregate = RealEstate.Property.Domain.Aggregates.Property;  
using RealEstate.Shared.Domain.ValueObjects;

namespace RealEstate.Property.Application.Mappers;

public static class PropertyMapper
{
    public static PropertyDto ToDto(this PropertyAggregate property)
    {
        return new PropertyDto
        {
            Id = property.Id.Id,
            City = property.Address.City,
            SubCity = property.Address.SubCity,
            Street = property.Address.Street,
            PriceAmount = property.Price.Amount,
            Currency = property.Price.Currency,
            Status = property.Status.ToString()
        };
    }
}
