using MediatR;

namespace PropertyManagement.Application.Commands;

public record UpdatePropertyPriceCommand(
    Guid PropertyId,
    decimal Price) : IRequest;

