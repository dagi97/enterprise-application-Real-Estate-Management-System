using MediatR;
using PropertyManagement.Application.Commands;
using PropertyManagement.Domain.Repositories;

namespace PropertyManagement.Application.Commands;

public class UpdatePropertyPriceCommandHandler : IRequestHandler<UpdatePropertyPriceCommand>
{
    private readonly IPropertyRepository _propertyRepository;

    public UpdatePropertyPriceCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task Handle(UpdatePropertyPriceCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        
        if (property == null)
            throw new InvalidOperationException($"Property with ID {request.PropertyId} not found");

        property.UpdatePrice(request.Price);

        await _propertyRepository.UpdateAsync(property, cancellationToken);
    }
}

