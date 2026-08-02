using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Domain.Events.Users;

namespace GM.Notifications.Sample.Consumer.Worker.Handlers;

public class UserConfirmedHandler(IInboxProcessor inbox, ILogger<UserConfirmedHandler> logger)
{
    public Task Handle(UserConfirmedIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.Notifications.Sample.Consumer.Worker] User confirmed — Subject: {Subject}, ConfirmationType: {ConfirmationType}",
            message.Subject, message.ConfirmationType);

        // Ingest only: the InboxProcessorWorker poller sends the notification for this row.
        return inbox.IngestAsync(message, "GM.Notifications.Sample.Consumer.Worker", ct);
    }
}