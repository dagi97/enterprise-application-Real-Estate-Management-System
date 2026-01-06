using MediatR;

namespace RealEstate.Property.Application.Commands.CancelReservation;

public sealed record CancelReservationCommand(
    Guid PropertyId
) : IRequest;

