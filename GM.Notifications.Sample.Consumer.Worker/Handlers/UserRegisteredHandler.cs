using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Domain.Events.Users;

namespace GM.Notifications.Sample.Consumer.Worker.Handlers;

public class UserRegisteredHandler(IInboxProcessor inbox, ILogger<UserRegisteredHandler> logger)
{
    public Task Handle(UserRegisteredIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.Notifications.Sample.Consumer.Worker] User registered — UserId: {UserId}, Email: {Email}, PhoneNumber: {PhoneNumber}",
            message.UserId, message.Email, message.PhoneNumber);

        // Ingest only: the InboxProcessorWorker poller sends the notification for this row.
        return inbox.IngestAsync(message, "GM.Notifications.Sample.Consumer.Worker", ct);
    }
}
