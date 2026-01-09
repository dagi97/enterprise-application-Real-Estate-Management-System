using MediatR;
using PropertyManagement.Application.DTOs;

namespace PropertyManagement.Application.Queries;

public record GetPropertyQuery(Guid PropertyId) : IRequest<PropertyDto?>;

