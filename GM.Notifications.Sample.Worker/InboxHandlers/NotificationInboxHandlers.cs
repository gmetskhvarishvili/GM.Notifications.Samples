using GM.Mediator.Contracts;
using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Application.Emails.Commands.SendEmail;
using GM.Notifications.Sample.Application.Smses.Commands.SendSms;
using GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsApp;
using GM.Notifications.Sample.Domain.Events.Otp;
using GM.Notifications.Sample.Domain.Events.Users;

namespace GM.Notifications.Sample.Worker.InboxHandlers;

// The consumer worker ingests these events; GM.Messaging's inbox worker calls these handlers for
// them. Each runs in one transaction with marking the inbox row handled, so the notification record
// the send command saves commits together with it. A failure rolls back and is retried with backoff,
// then dead-lettered (Messaging:Inbox settings).

/// <summary>Sends the welcome email when a user registers.</summary>
public sealed class UserRegisteredInboxHandler(IMediator mediator) : IInboxHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var email = integrationEvent.Email
                    ?? throw new InvalidOperationException("UserRegisteredIntegrationEvent.Email is required.");

        await mediator.Send(new SendEmailCommand(
            email,
            "Welcome to GM Notifications Sample",
            $"Hello {integrationEvent.Username},\n\nThank you for registering!",
            UserId: integrationEvent.UserId), cancellationToken);
    }
}

/// <summary>Confirms a user's email or phone number over the channel they confirmed.</summary>
public sealed class UserConfirmedInboxHandler(IMediator mediator) : IInboxHandler<UserConfirmedIntegrationEvent>
{
    public async Task HandleAsync(UserConfirmedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var subject = integrationEvent.Subject
                      ?? throw new InvalidOperationException("UserConfirmedIntegrationEvent.Subject is required.");

        switch (integrationEvent.ConfirmationType)
        {
            case 0: // Email confirmation
                await mediator.Send(new SendEmailCommand(
                    subject,
                    "Email Confirmed",
                    $"Hello {subject},\n\nThank you for confirming email!",
                    UserId: integrationEvent.UserId), cancellationToken);
                break;
            case 1: // SMS confirmation
                await mediator.Send(new SendSmsCommand(subject, "Phone Number Confirmed", UserId: integrationEvent.UserId), cancellationToken);
                break;
            case 2: // WhatsApp confirmation
                await mediator.Send(new SendWhatsAppCommand(subject, "Phone Number Confirmed", UserId: integrationEvent.UserId), cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown confirmation type: {integrationEvent.ConfirmationType}");
        }
    }
}

/// <summary>Delivers a one-time code GM.OTP generated; the message already contains the code.</summary>
public sealed class OtpGeneratedInboxHandler(IMediator mediator) : IInboxHandler<OtpGeneratedIntegrationEvent>
{
    public async Task HandleAsync(OtpGeneratedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        // Channel carries the OTP delivery channel (GM.OTP OtpChannel: Sms = 1, Email = 2).
        switch (integrationEvent.Channel)
        {
            case 2: // WhatsApp
                await mediator.Send(new SendWhatsAppCommand(integrationEvent.Destination, integrationEvent.Message, UserId: integrationEvent.UserId), cancellationToken);
                break;
            case 1: // SMS
                await mediator.Send(new SendSmsCommand(integrationEvent.Destination, integrationEvent.Message, UserId: integrationEvent.UserId), cancellationToken);
                break;
            case 0: // Email
                await mediator.Send(new SendEmailCommand(
                    integrationEvent.Destination,
                    "Your one-time code",
                    integrationEvent.Message,
                    UserId: integrationEvent.UserId), cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown OTP channel: {integrationEvent.Channel}");
        }
    }
}
