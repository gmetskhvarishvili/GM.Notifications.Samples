using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Domain.Events.Otp;

namespace GM.Notifications.Sample.Consumer.Worker.Handlers;

public class OtpGeneratedHandler(IInboxProcessor inbox, ILogger<OtpGeneratedHandler> logger)
{
    public Task Handle(OtpGeneratedIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.Notifications.Sample.Consumer.Worker] OTP generated — Destination: {Destination}, Channel: {Channel}, Purpose: {Purpose}",
            message.Destination, message.Channel, message.Purpose);

        // Ingest only: the InboxProcessorWorker poller sends the notification for this row.
        return inbox.IngestAsync(message, "GM.Notifications.Sample.Consumer.Worker", ct);
    }
}
