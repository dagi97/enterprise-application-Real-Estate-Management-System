using MediatR;
using RealEstate.Property.Domain.Events;

namespace RealEstate.Property.Application.Notifications;

public sealed record PropertyWithdrawnNotification(PropertyWithdrawn DomainEvent) : INotification;


