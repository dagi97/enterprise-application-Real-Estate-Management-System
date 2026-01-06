namespace RealEstate.Property.Infrastructure.Messaging;

public interface IOutboxPublisher
{
    Task PublishPendingEventsAsync(CancellationToken cancellationToken = default);
}

